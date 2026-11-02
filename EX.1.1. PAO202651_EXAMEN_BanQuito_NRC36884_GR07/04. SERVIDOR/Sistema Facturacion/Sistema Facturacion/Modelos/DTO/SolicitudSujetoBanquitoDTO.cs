namespace SISTEMA_FACTURACION.Modelos.DTO;
public sealed class SolicitudSujetoBanquitoDTO { public string Cedula { get; set; } = string.Empty; }
public sealed class RespuestaSujetoBanquitoDTO { public bool SujetoCredito { get; set; } public int? CodCliente { get; set; } public string? Motivo { get; set; } }
public sealed class SolicitudMontoBanquitoDTO { public int CodCliente { get; set; } }
public sealed class RespuestaMontoBanquitoDTO { public int CodCliente { get; set; } public decimal MontoMaximo { get; set; } }
public sealed class SolicitudOtorgarBanquitoDTO { public int CodCliente { get; set; } public decimal PrecioTelefono { get; set; } public int NumCuotas { get; set; } }
public sealed class RespuestaOtorgarBanquitoDTO { public int IdCredito { get; set; } public bool Aprobado { get; set; } public string? Motivo { get; set; } }
