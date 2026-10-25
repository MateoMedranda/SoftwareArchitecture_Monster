namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Conversiones;

/// <summary>Resultado de una conversión, independiente del formato SOAP.</summary>
public record ResultadoConversionModelo(bool Exito, double? Valor, string Mensaje);
