using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SISTEMA_FACTURACION.Modelos;

/// <summary>
/// Conexion a SQL Server de la base de facturacion.
/// </summary>
public sealed class Conexion
{
    private const string NombreCadenaConexion = "Facturacion";

    private readonly string _cadenaConexion;

    public Conexion(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString(NombreCadenaConexion)
            ?? throw new InvalidOperationException(
                $"No se encontro la cadena de conexion '{NombreCadenaConexion}'.");
    }

    public string CadenaConexion => _cadenaConexion;

    public string NombreBaseDatos =>
        new SqlConnectionStringBuilder(_cadenaConexion).InitialCatalog;

    /// <summary>
    /// Crea una nueva conexion lista para abrirse. Cada DAO usa su propia conexion.
    /// </summary>
    public SqlConnection CrearConexion() => new(_cadenaConexion);

    /// <summary>
    /// Ejecuta SELECT 1 para comprobar que la base de datos responde.
    /// </summary>
    public async Task<bool> ProbarConexionAsync(CancellationToken cancelacion = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancelacion);

        await using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT 1";

        var resultado = await comando.ExecuteScalarAsync(cancelacion);
        return Convert.ToInt32(resultado) == 1;
    }
}
