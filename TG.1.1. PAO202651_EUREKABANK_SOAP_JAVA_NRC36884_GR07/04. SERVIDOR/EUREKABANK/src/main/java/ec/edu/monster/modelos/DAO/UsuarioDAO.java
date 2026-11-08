package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.UsuarioDTO;
import jakarta.ejb.EJB;
import jakarta.ejb.Stateless;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Stateless
public class UsuarioDAO {
    @EJB private ConexionBDD conexionBDD;

    public void insertar(UsuarioDTO u) throws SQLException {
        String sql = "INSERT INTO USUARIO (USUARIO_ID,ROL_ID,USUARIO_NOMBRE,USUARIO_APELLIDO,USUARIO_TELEFONO,USUARIO_USUARIO,USUARIO_CONTRASEANIA) VALUES (?,?,?,?,?,?,?)";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1,u.getUsuarioId()); ps.setString(2,u.getRolId());
            ps.setString(3,u.getUsuarioNombre()); ps.setString(4,u.getUsuarioApellido());
            ps.setString(5,u.getUsuarioTelefono()); ps.setString(6,u.getUsuarioUsuario());
            ps.setString(7,u.getUsuarioContraseania());
            ps.executeUpdate();
        }
    }

    public UsuarioDTO obtenerPorId(String id) throws SQLException {
        String sql = "SELECT USUARIO_ID,ROL_ID,USUARIO_NOMBRE,USUARIO_APELLIDO,USUARIO_TELEFONO,USUARIO_USUARIO,USUARIO_CONTRASEANIA FROM USUARIO WHERE USUARIO_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id);
            try (ResultSet rs = ps.executeQuery()) { if (rs.next()) return mapear(rs); }
        }
        return null;
    }

    public List<UsuarioDTO> listar() throws SQLException {
        List<UsuarioDTO> lista = new ArrayList<>();
        String sql = "SELECT USUARIO_ID,ROL_ID,USUARIO_NOMBRE,USUARIO_APELLIDO,USUARIO_TELEFONO,USUARIO_USUARIO,USUARIO_CONTRASEANIA FROM USUARIO";
        try (Connection con = conexionBDD.getConexion(); Statement st = con.createStatement(); ResultSet rs = st.executeQuery(sql)) {
            while (rs.next()) lista.add(mapear(rs));
        }
        return lista;
    }

    public void actualizar(UsuarioDTO u) throws SQLException {
        String sql = "UPDATE USUARIO SET ROL_ID=?,USUARIO_NOMBRE=?,USUARIO_APELLIDO=?,USUARIO_TELEFONO=?,USUARIO_USUARIO=?,USUARIO_CONTRASEANIA=? WHERE USUARIO_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1,u.getRolId()); ps.setString(2,u.getUsuarioNombre());
            ps.setString(3,u.getUsuarioApellido()); ps.setString(4,u.getUsuarioTelefono());
            ps.setString(5,u.getUsuarioUsuario()); ps.setString(6,u.getUsuarioContraseania());
            ps.setString(7,u.getUsuarioId());
            ps.executeUpdate();
        }
    }

    public void eliminar(String id) throws SQLException {
        String sql = "DELETE FROM USUARIO WHERE USUARIO_ID=?";
        try (Connection con = conexionBDD.getConexion(); PreparedStatement ps = con.prepareStatement(sql)) {
            ps.setString(1, id); ps.executeUpdate();
        }
    }

    private UsuarioDTO mapear(ResultSet rs) throws SQLException {
        UsuarioDTO u = new UsuarioDTO();
        u.setUsuarioId(rs.getString("USUARIO_ID"));
        u.setRolId(rs.getString("ROL_ID"));
        u.setUsuarioNombre(rs.getString("USUARIO_NOMBRE"));
        u.setUsuarioApellido(rs.getString("USUARIO_APELLIDO"));
        u.setUsuarioTelefono(rs.getString("USUARIO_TELEFONO"));
        u.setUsuarioUsuario(rs.getString("USUARIO_USUARIO"));
        u.setUsuarioContraseania(rs.getString("USUARIO_CONTRASEANIA"));
        return u;
    }
}
