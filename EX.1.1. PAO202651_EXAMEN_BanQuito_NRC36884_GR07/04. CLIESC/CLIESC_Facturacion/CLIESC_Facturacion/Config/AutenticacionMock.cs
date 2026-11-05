namespace CLIESC_Facturacion.Config
{
    public static class AutenticacionMock
    {
        public const string Token = "MONSTER-DEMO-TOKEN";
        public static bool CredencialesValidas(string usuario, string contrasena) =>
            usuario == "MONSTER" && contrasena == "MONSTER9";
    }
}
