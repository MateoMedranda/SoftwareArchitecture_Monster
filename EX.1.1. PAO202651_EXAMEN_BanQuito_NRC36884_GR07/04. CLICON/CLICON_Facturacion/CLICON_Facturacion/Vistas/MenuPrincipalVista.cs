using System.Threading.Tasks;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Menú principal del cliente de consola.
    /// </summary>
    public sealed class MenuPrincipalVista
    {
        public async Task MostrarAsync()
        {
            while (true)
            {
                Consola.Limpiar();
                LogoPixelArt.Mostrar();

                Consola.MenuOpcion(1, "Facturas");
                Consola.MenuOpcion(2, "Clientes");
                Consola.MenuOpcion(3, "Catálogo");
                Consola.MenuOpcion(4, "Salir");

                var opcion = Consola.LeerOpcion("   Seleccione una opción: ", 1, 4);
                switch (opcion)
                {
                    case 1:
                        await new FacturasVista().MostrarAsync();
                        break;
                    case 2:
                        await new ClientesVista().MostrarAsync();
                        break;
                    case 3:
                        await new CatalogoVista().MostrarAsync();
                        break;
                    case 4:
                        Consola.Limpiar();
                        Consola.Exito("Gracias por usar Celulares Monster.");
                        return;
                }
            }
        }
    }
}
