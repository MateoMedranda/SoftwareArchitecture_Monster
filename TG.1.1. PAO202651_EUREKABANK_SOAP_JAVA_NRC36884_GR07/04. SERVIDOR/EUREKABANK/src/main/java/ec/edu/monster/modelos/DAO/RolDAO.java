package ec.edu.monster.modelos.DAO;

import ec.edu.monster.conexion.ConexionBDD;
import ec.edu.monster.modelos.DTO.RolDTO;
import jakarta.ejb.EJB; import jakarta.ejb.Stateless;
import java.sql.*;

@Stateless
public class RolDAO {
    @EJB private ConexionBDD conexionBDD;
    public RolDTO obtenerPorId(String id) throws SQLException {
        String sql="SELECT ROL_ID,ROL_NOMBRE FROM ROL WHERE ROL_ID=?";
        try(Connection con=conexionBDD.getConexion(); PreparedStatement ps=con.prepareStatement(sql)){
            ps.setString(1,id);
            try(ResultSet rs=ps.executeQuery()){ if(rs.next()){ RolDTO r=new RolDTO(); r.setRolId(rs.getString("ROL_ID")); r.setRolNombre(rs.getString("ROL_NOMBRE")); return r; } }
        }
        return null;
    }
    public RolDTO obtenerPorNombre(String nombre) throws SQLException {
        String sql="SELECT ROL_ID,ROL_NOMBRE FROM ROL WHERE ROL_NOMBRE=?";
        try(Connection con=conexionBDD.getConexion(); PreparedStatement ps=con.prepareStatement(sql)){
            ps.setString(1,nombre);
            try(ResultSet rs=ps.executeQuery()){ if(rs.next()){ RolDTO r=new RolDTO(); r.setRolId(rs.getString("ROL_ID")); r.setRolNombre(rs.getString("ROL_NOMBRE")); return r; } }
        }
        return null;
    }
}
