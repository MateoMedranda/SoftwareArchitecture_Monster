using System;
using CLICON_Facturacion.Config;
using CLICON_Facturacion.Vistas;

namespace CLICON_Facturacion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Consola.Inicializar();
            try
            {
                AppConfig.Load();
                new MenuPrincipalVista().MostrarAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Consola.Error("Ocurrió un error inesperado: " + ex.Message);
                Consola.Pausa();
            }
            finally
            {
                Consola.Restaurar();
            }
        }
    }
}
