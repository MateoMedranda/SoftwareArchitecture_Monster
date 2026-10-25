using System.ComponentModel.DataAnnotations;

namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

/// <summary>Modelo del formulario de conversión (MODELO de MVC).</summary>
public class SolicitudConversionModelo
{
    public string Categoria { get; set; } = CatalogoUnidades.Categorias[0];

    [Required(ErrorMessage = "Ingrese un valor numérico.")]
    public decimal? Valor { get; set; }

    [Required(ErrorMessage = "Seleccione la unidad inicial.")]
    public string UnidadOrigen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione la unidad final.")]
    public string UnidadDestino { get; set; } = string.Empty;
}
