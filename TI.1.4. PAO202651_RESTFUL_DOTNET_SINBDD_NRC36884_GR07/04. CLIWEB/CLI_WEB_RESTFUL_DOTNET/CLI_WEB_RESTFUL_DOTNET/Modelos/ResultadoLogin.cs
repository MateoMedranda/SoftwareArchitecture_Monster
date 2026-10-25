namespace CLI_WEB_RESTFUL_DOTNET.Modelos;

/// <summary>Resultado de un intento de inicio de sesión, independiente del formato SOAP.</summary>
public record ResultadoLogin(bool Exito, string? Token, string Mensaje);
