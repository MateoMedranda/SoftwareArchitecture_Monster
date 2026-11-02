namespace SISTEMA_FACTURACION.Comun;
public sealed class ExcepcionNegocio(int statusCode, string message) : Exception(message) { public int StatusCode { get; } = statusCode; }
