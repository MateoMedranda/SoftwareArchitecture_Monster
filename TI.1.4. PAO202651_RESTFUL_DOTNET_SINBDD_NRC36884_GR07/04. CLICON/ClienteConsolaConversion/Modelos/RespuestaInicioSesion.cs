namespace ClienteConsolaConversion.Modelos
{
    public class RespuestaInicioSesion
    {
        public bool Exito { get; set; }
        public string? Token { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
