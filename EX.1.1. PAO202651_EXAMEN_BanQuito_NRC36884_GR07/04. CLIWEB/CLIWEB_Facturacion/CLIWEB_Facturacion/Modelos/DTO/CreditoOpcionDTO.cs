namespace CLIWEB_Facturacion.Modelos.DTO;

/// <summary>Opción del selector de créditos de la tienda.</summary>
public sealed class CreditoOpcionDTO
{
    public int NumCredito { get; set; }

    public int? NumFactura { get; set; }

    public string Cedula { get; set; } = string.Empty;

    public string NombreCliente { get; set; } = string.Empty;

    public DateTime? Fecha { get; set; }

    public decimal? Valor { get; set; }

    public string Origen { get; set; } = "Tienda";

    /// <summary>Texto que se muestra en el selector de créditos.</summary>
    public string Etiqueta
    {
        get
        {
            var partes = new List<string> { $"Crédito N° {NumCredito}" };

            if (!string.IsNullOrWhiteSpace(NombreCliente))
            {
                partes.Add(NombreCliente);
            }

            if (NumFactura is not null)
            {
                partes.Add($"Factura N° {NumFactura}");
            }

            if (Valor is not null)
            {
                partes.Add($"$ {Valor.Value:N2}");
            }

            partes.Add(Origen);
            return string.Join(" · ", partes);
        }
    }
}
