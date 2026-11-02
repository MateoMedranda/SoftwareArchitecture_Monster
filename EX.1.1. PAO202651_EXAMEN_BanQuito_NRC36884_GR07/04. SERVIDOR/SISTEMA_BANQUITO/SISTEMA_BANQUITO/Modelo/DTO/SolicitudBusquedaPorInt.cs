using System.ComponentModel.DataAnnotations;

namespace SISTEMA_BANQUITO.Modelo.DTO;

/// <summary>
/// Solicitud generica para buscar un registro por un valor entero.
/// Sirve para codigo de cliente, codigo de movimiento o numero de credito.
/// </summary>
public sealed class SolicitudBusquedaPorInt
{
    [Range(1, int.MaxValue, ErrorMessage = "El valor de busqueda debe ser mayor que cero.")]
    public int Valor { get; set; }
}
