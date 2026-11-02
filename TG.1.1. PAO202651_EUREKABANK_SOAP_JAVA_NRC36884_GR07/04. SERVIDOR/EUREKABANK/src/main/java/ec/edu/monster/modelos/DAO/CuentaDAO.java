package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.math.BigDecimal;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class CuentaDAO {
    @EJB private ConexionBDD conexionBDD;

    public void insertar(CuentaDTO c) throws SQLException {
        String sql = "INSERT INTO CUENTA (CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO) VALUES (?,?,?,?,?)";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, c.getCuentaId()); ps.setString(2, c.getCuentaNumero()); ps.setString(3, c.getSucursalId());
            ps.setBigDecimal(4, c.getCuentaSaldo()); ps.setString(5, c.getCuentaEstado());
            ps.executeUpdate();
        }
    }

    /**
     * Crea y asigna una cuenta en una única transacción JDBC. El número público
     * se deriva en el servidor y se respalda con la restricción UNIQUE de BDD.
     */
    public CuentaDTO crearYAsignarAutomaticamente(String usuarioId) throws SQLException {
        SQLException ultimoError = null;
        for (int intento = 0; intento < 5; intento++) {
            try (Connection con = conexionBDD.getConexion()) {
                con.setAutoCommit(false);
                try {
                    String sucursalId = obtenerSucursalPredeterminada(con);
                    if (sucursalId == null) throw new SQLException("No existe una sucursal para asignar la cuenta.");
                    String numero = siguienteNumeroPublico(con);
                    CuentaDTO cuenta = new CuentaDTO();
                    cuenta.setCuentaId(java.util.UUID.randomUUID().toString());
                    cuenta.setCuentaNumero(numero);
                    cuenta.setSucursalId(sucursalId);
                    cuenta.setCuentaSaldo(BigDecimal.ZERO);
                    cuenta.setCuentaEstado("ACTIVA");
                    insertar(con, cuenta);
                    try (PreparedStatement ps = con.prepareStatement(
                            "INSERT INTO PERTENECE (CUENTA_ID,USUARIO_ID) VALUES (?,?)")) {
                        ps.setString(1, cuenta.getCuentaId());
                        ps.setString(2, usuarioId);
                        ps.executeUpdate();
                    }
                    con.commit();
                    return cuenta;
                } catch (SQLException e) {
                    con.rollback();
                    ultimoError = e;
                    if (!esNumeroDuplicado(e)) throw e;
                } finally {
                    try { con.setAutoCommit(true); } catch (SQLException ignored) { }
                }
            }
        }
        throw ultimoError == null ? new SQLException("No fue posible generar un número de cuenta.") : ultimoError;
    }

    private String obtenerSucursalPredeterminada(Connection con) throws SQLException {
        try (PreparedStatement ps = con.prepareStatement(
                "SELECT SUCURSAL_ID FROM SUCURSAL ORDER BY SUCURSAL_ID LIMIT 1");
             ResultSet rs = ps.executeQuery()) {
            return rs.next() ? rs.getString("SUCURSAL_ID") : null;
        }
    }

    private String siguienteNumeroPublico(Connection con) throws SQLException {
        String sql = "SELECT COALESCE(MAX(CAST(CUENTA_NUMERO AS UNSIGNED)), 1000000000) AS ULTIMO FROM CUENTA";
        try (PreparedStatement ps = con.prepareStatement(sql); ResultSet rs = ps.executeQuery()) {
            if (!rs.next()) throw new SQLException("No se pudo calcular el siguiente número de cuenta.");
            long siguiente = Math.max(1_000_000_000L, rs.getLong("ULTIMO")) + 1;
            if (siguiente > 9_999_999_999L) throw new SQLException("Se agotaron los números de cuenta disponibles.");
            return String.format("%010d", siguiente);
        }
    }

    private void insertar(Connection con, CuentaDTO c) throws SQLException {
        String sql = "INSERT INTO CUENTA (CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO) VALUES (?,?,?,?,?)";
        try (PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, c.getCuentaId()); ps.setString(2, c.getCuentaNumero()); ps.setString(3, c.getSucursalId());
            ps.setBigDecimal(4, c.getCuentaSaldo()); ps.setString(5, c.getCuentaEstado());
            ps.executeUpdate();
        }
    }

    private boolean esNumeroDuplicado(SQLException e) {
        return "23000".equals(e.getSQLState()) && e.getMessage() != null
                && e.getMessage().contains("UK_CUENTA_NUMERO");
    }

    public CuentaDTO obtenerPorId(String id) throws SQLException {
        String sql = "SELECT CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO FROM CUENTA WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id);
            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) return mapear(rs);
            }
        }
        return null;
    }

    public CuentaDTO obtenerPorNumero(String numero) throws SQLException {
        String sql = "SELECT CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO FROM CUENTA WHERE CUENTA_NUMERO=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, numero);
            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) return mapear(rs);
            }
        }
        return null;
    }

    /** Acepta temporalmente UUID interno o número público para compatibilidad SOAP. */
    public String resolverId(String referencia) throws SQLException {
        if (referencia == null || referencia.trim().isEmpty()) return null;
        String valor = referencia.trim();
        CuentaDTO cuenta = obtenerPorId(valor);
        if (cuenta == null) cuenta = obtenerPorNumero(valor);
        return cuenta == null ? null : cuenta.getCuentaId();
    }

    /**
     * Obtiene una cuenta dentro de una transacción ya iniciada y bloquea su fila
     * hasta que se confirme o revierta dicha transacción.
     */
    public CuentaDTO obtenerPorIdParaActualizar(Connection con, String id) throws SQLException {
        String sql = "SELECT CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO "
                   + "FROM CUENTA WHERE CUENTA_ID=? FOR UPDATE";
        try (PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id);
            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) return mapear(rs);
            }
        }
        return null;
    }

    public List<CuentaDTO> listar() throws SQLException {
        List<CuentaDTO> lista = new ArrayList<>();
        String sql = "SELECT CUENTA_ID,CUENTA_NUMERO,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO FROM CUENTA";
        try (Connection con = conexionBDD.getConexion(); Statement st = con.createStatement(); ResultSet rs = st.executeQuery(sql)) {
            while (rs.next()) lista.add(mapear(rs));
        }
        return lista;
    }

    public void actualizar(CuentaDTO c) throws SQLException {
        String sql = "UPDATE CUENTA SET SUCURSAL_ID=?,CUENTA_SALDO=?,CUENTA_ESTADO=? WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, c.getSucursalId()); ps.setBigDecimal(2, c.getCuentaSaldo());
            ps.setString(3, c.getCuentaEstado()); ps.setString(4, c.getCuentaId());
            ps.executeUpdate();
        }
    }

    /** Actualiza exclusivamente el saldo dentro de la transacción del llamador. */
    public void actualizarSaldo(Connection con, String cuentaId, BigDecimal saldo) throws SQLException {
        String sql = "UPDATE CUENTA SET CUENTA_SALDO=? WHERE CUENTA_ID=?";
        try (PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setBigDecimal(1, saldo);
            ps.setString(2, cuentaId);
            if (ps.executeUpdate() != 1) {
                throw new SQLException("No se pudo actualizar el saldo de la cuenta " + cuentaId);
            }
        }
    }

    /**
     * Evita que operaciones administrativas alteren el saldo sin dejar un
     * movimiento contable. El saldo solo cambia mediante una transacción.
     */
    public void actualizarDatosAdministrativos(CuentaDTO c) throws SQLException {
        String sql = "UPDATE CUENTA SET SUCURSAL_ID=?,CUENTA_ESTADO=? WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, c.getSucursalId());
            ps.setString(2, c.getCuentaEstado());
            ps.setString(3, c.getCuentaId());
            ps.executeUpdate();
        }
    }

    public void eliminar(String id) throws SQLException {
        String sql = "DELETE FROM CUENTA WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id); ps.executeUpdate();
        }
    }

    private CuentaDTO mapear(ResultSet rs) throws SQLException {
        CuentaDTO c = new CuentaDTO();
        c.setCuentaId(rs.getString("CUENTA_ID"));
        c.setCuentaNumero(rs.getString("CUENTA_NUMERO"));
        c.setSucursalId(rs.getString("SUCURSAL_ID"));
        c.setCuentaSaldo(rs.getBigDecimal("CUENTA_SALDO"));
        c.setCuentaEstado(rs.getString("CUENTA_ESTADO"));
        return c;
    }
}
