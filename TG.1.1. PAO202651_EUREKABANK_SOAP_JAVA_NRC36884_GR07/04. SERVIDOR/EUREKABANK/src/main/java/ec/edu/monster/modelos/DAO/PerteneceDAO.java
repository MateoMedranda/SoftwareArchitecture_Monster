package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.PerteneceDTO;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class PerteneceDAO {
    @EJB private ConexionBDD conexionBDD;

    public void insertar(PerteneceDTO p) throws SQLException {
        String sql = "INSERT INTO PERTENECE (CUENTA_ID,USUARIO_ID) VALUES (?,?)";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, p.getCuentaId()); ps.setString(2, p.getUsuarioId());
            ps.executeUpdate();
        }
    }

    public List<PerteneceDTO> listarPorCuenta(String cuentaId) throws SQLException {
        List<PerteneceDTO> lista = new ArrayList<>();
        String sql = "SELECT CUENTA_ID,USUARIO_ID FROM PERTENECE WHERE CUENTA_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, cuentaId);
            try (ResultSet rs = ps.executeQuery()) { while (rs.next()) lista.add(mapear(rs)); }
        }
        return lista;
    }

    public List<PerteneceDTO> listarPorUsuario(String usuarioId) throws SQLException {
        List<PerteneceDTO> lista = new ArrayList<>();
        String sql = "SELECT CUENTA_ID,USUARIO_ID FROM PERTENECE WHERE USUARIO_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, usuarioId);
            try (ResultSet rs = ps.executeQuery()) { while (rs.next()) lista.add(mapear(rs)); }
        }
        return lista;
    }

    public void eliminar(String cuentaId, String usuarioId) throws SQLException {
        String sql = "DELETE FROM PERTENECE WHERE CUENTA_ID=? AND USUARIO_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, cuentaId); ps.setString(2, usuarioId);
            ps.executeUpdate();
        }
    }

    private PerteneceDTO mapear(ResultSet rs) throws SQLException {
        PerteneceDTO p = new PerteneceDTO();
        p.setCuentaId(rs.getString("CUENTA_ID"));
        p.setUsuarioId(rs.getString("USUARIO_ID"));
        return p;
    }
}
