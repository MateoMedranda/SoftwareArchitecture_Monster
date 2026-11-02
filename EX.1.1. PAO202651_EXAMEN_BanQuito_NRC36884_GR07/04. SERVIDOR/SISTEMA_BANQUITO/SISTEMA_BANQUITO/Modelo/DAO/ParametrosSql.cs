using System.Data;
using Microsoft.Data.SqlClient;

namespace SISTEMA_BANQUITO.Modelo.DAO;

/// <summary>
/// Utilidades para crear parametros de SQL Server con la precision
/// correcta en las columnas monetarias.
/// </summary>
internal static class ParametrosSql
{
    /// <summary>
    /// Agrega un parametro decimal con precision 10 y escala 2.
    /// Sin Precision/Scale, SqlClient puede redondear el valor a entero.
    /// </summary>
    public static void AgregarDecimal(
        this SqlParameterCollection parametros,
        string nombre,
        decimal valor)
    {
        var parametro = parametros.Add(nombre, SqlDbType.Decimal);
        parametro.Precision = 10;
        parametro.Scale = 2;
        parametro.Value = valor;
    }
}
