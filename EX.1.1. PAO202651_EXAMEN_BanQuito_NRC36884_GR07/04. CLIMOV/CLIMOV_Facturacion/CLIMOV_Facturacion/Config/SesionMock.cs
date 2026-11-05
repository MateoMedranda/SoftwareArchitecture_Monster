namespace CLIMOV_Facturacion.Config;

/// <summary>Sesión local de demostración; no usa ni almacena usuarios reales.</summary>
public sealed class SesionMock
{
    public const string UsuarioDemo = "MONSTER";
    public const string ContrasenaDemo = "MONSTER9";
    public const string TokenDemo = "MONSTER-DEMO-TOKEN";

    public bool Autenticado { get; private set; }

    public bool Iniciar(string? usuario, string? contrasena)
    {
        Autenticado = string.Equals(usuario, UsuarioDemo, StringComparison.Ordinal) &&
                      string.Equals(contrasena, ContrasenaDemo, StringComparison.Ordinal);
        return Autenticado;
    }

    public void Cerrar() => Autenticado = false;
}
