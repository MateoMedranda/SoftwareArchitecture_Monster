package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.TransaccionDTO;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class TransaccionDAO {
    @EJB private ConexionBDD conexionBDD;

    public void insertar(TransaccionDTO t) throws SQLException {
        String sql = "INSERT INTO TRANSACCION (TRANSACCION_ID,CUENTA_ID,TRANSACCION_SALDO,TRANSACCION_TIPO,TRANSACCION_SALDO_ANTERIOR,TRANSACCION_SALDO_POSTERIOR,TRANSACCION_CREADO_EN,TRANSACCION_CREADO_POR) VALUES (?,?,?,?,?,?,?,?)";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1,t.getTransaccionId()); ps.setString(2,t.getCuentaId());
            ps.setBigDecimal(3,t.getTransaccionSaldo()); ps.setString(4,t.getTransaccionTipo());
            ps.setBigDecimal(5,t.getTransaccionSaldoAnterior()); ps.setBigDecimal(6,t.getTransaccionSaldoPosterior());
            ps.setTimestamp(7, Timestamp.valueOf(t.getTransaccionCreadoEn()));
            ps.setString(8,t.getTransaccionCreadoPor());
            ps.executeUpdate();
        }
    }

    public TransaccionDTO obtenerPorId(String id) throws SQLException {
        String sql = "SELECT TRANSACCION_ID,CUENTA_ID,TRANSACCION_SALDO,TRANSACCION_TIPO,TRANSACCION_SALDO_ANTERIOR,TRANSACCION_SALDO_POSTERIOR,TRANSACCION_CREADO_EN,TRANSACCION_CREADO_POR FROM TRANSACCION WHERE TRANSACCION_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id);
            try (ResultSet rs = ps.executeQuery()) { if (rs.next()) return mapear(rs); }
        }
        return null;
    }

    public List<TransaccionDTO> listarPorCuenta(String cuentaId) throws SQLException {
        List<TransaccionDTO> lista = new ArrayList<>();
        String sql = "SELECT TRANSACCION_ID,CUENTA_ID,TRANSACCION_SALDO,TRANSACCION_TIPO,TRANSACCION_SALDO_ANTERIOR,TRANSACCION_SALDO_POSTERIOR,TRANSACCION_CREADO_EN,TRANSACCION_CREADO_POR FROM TRANSACCION WHERE CUENTA_ID=? ORDER BY TRANSACCION_CREADO_EN DESC";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, cuentaId);
            try (ResultSet rs = ps.executeQuery()) { while (rs.next()) lista.add(mapear(rs)); }
        }
        return lista;
    }

    public List<TransaccionDTO> listar() throws SQLException {
        List<TransaccionDTO> lista = new ArrayList<>();
        String sql = "SELECT TRANSACCION_ID,CUENTA_ID,TRANSACCION_SALDO,TRANSACCION_TIPO,TRANSACCION_SALDO_ANTERIOR,TRANSACCION_SALDO_POSTERIOR,TRANSACCION_CREADO_EN,TRANSACCION_CREADO_POR FROM TRANSACCION";
        try (Connection con = conexionBDD.getConexion(); Statement st = con.createStatement(); ResultSet rs = st.executeQuery(sql)) {
            while (rs.next()) lista.add(mapear(rs));
        }
        return lista;
    }

    private TransaccionDTO mapear(ResultSet rs) throws SQLException {
        TransaccionDTO t = new TransaccionDTO();
        t.setTransaccionId(rs.getString("TRANSACCION_ID"));
        t.setCuentaId(rs.getString("CUENTA_ID"));
        t.setTransaccionSaldo(rs.getBigDecimal("TRANSACCION_SALDO"));
        t.setTransaccionTipo(rs.getString("TRANSACCION_TIPO"));
        t.setTransaccionSaldoAnterior(rs.getBigDecimal("TRANSACCION_SALDO_ANTERIOR"));
        t.setTransaccionSaldoPosterior(rs.getBigDecimal("TRANSACCION_SALDO_POSTERIOR"));
        Timestamp ts = rs.getTimestamp("TRANSACCION_CREADO_EN");
        t.setTransaccionCreadoEn(ts != null ? ts.toLocalDateTime() : null);
        t.setTransaccionCreadoPor(rs.getString("TRANSACCION_CREADO_POR"));
        return t;
    }
    public void eliminar(String id) throws SQLException {
        String sql="DELETE FROM TRANSACCION WHERE TRANSACCION_ID=?";
        try(Connection con=conexionBDD.getConexion(); PreparedStatement ps=con.prepareStatement(sql)){
            ps.setString(1,id); ps.executeUpdate();
        }
    }
}
