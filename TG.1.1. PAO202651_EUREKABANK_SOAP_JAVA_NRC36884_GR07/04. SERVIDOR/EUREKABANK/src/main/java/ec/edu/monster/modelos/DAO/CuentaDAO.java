package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.CuentaDTO;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class CuentaDAO {
    @EJB private ConexionBDD conexionBDD;

    public void insertar(CuentaDTO c) throws SQLException {
        String sql = "INSERT INTO CUENTA (CUENTA_ID,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO) VALUES (?,?,?,?)";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, c.getCuentaId()); ps.setString(2, c.getSucursalId());
            ps.setBigDecimal(3, c.getCuentaSaldo()); ps.setString(4, c.getCuentaEstado());
            ps.executeUpdate();
        }
    }

    public CuentaDTO obtenerPorId(String id) throws SQLException {
        String sql = "SELECT CUENTA_ID,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO FROM CUENTA WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id);
            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) return mapear(rs);
            }
        }
        return null;
    }

    /**
     * Obtiene una cuenta dentro de una transacción ya iniciada y bloquea su fila
     * hasta que se confirme o revierta dicha transacción.
     */
    public CuentaDTO obtenerPorIdParaActualizar(Connection con, String id) throws SQLException {
        String sql = "SELECT CUENTA_ID,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO "
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
        String sql = "SELECT CUENTA_ID,SUCURSAL_ID,CUENTA_SALDO,CUENTA_ESTADO FROM CUENTA";
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
        c.setSucursalId(rs.getString("SUCURSAL_ID"));
        c.setCuentaSaldo(rs.getBigDecimal("CUENTA_SALDO"));
        c.setCuentaEstado(rs.getString("CUENTA_ESTADO"));
        return c;
    }
}
