namespace ClienteEscritorioConversion.Modelos
{
    /// <summary>
    /// Token de la sesión en memoria del cliente.
    /// Nunca se persiste en disco y se envía en cada llamada a Convertir.
    /// </summary>
    public class SesionActual
    {
        public string? Token { get; private set; }

        public string? NombreUsuario { get; private set; }

        public bool Autenticado => !string.IsNullOrEmpty(Token);

        public void Iniciar(string nombreUsuario, string token)
        {
            NombreUsuario = nombreUsuario;
            Token = token;
        }

        public void Cerrar()
        {
            NombreUsuario = null;
            Token = null;
        }
    }
}
