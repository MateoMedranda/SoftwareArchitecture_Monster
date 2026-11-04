using System;
using System.Threading.Tasks;
using CLICON_Facturacion.Controladores;
using CLICON_Facturacion.Modelos;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Vistas de consulta y mantenimiento del catálogo de productos.
    /// </summary>
    public sealed class CatalogoVista
    {
        private readonly ProductoControlador _controlador;

        public CatalogoVista()
        {
            _controlador = new ProductoControlador();
        }

        public async Task MostrarAsync()
        {
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Catálogo");
                Consola.MenuOpcion(1, "Agregar producto");
                Consola.MenuOpcion(2, "Ver catálogo completo");
                Consola.MenuOpcion(3, "Buscar producto");
                Consola.MenuOpcion(4, "Actualizar datos de producto");
                Consola.MenuOpcion(5, "Eliminar producto");
                Consola.MenuOpcion(6, "Atrás");

                var opcion = Consola.LeerOpcion("   Seleccione una opción: ", 1, 6);
                switch (opcion)
                {
                    case 1:
                        await AgregarAsync();
                        break;
                    case 2:
                        await ListarAsync();
                        break;
                    case 3:
                        await BuscarAsync();
                        break;
                    case 4:
                        await ActualizarAsync();
                        break;
                    case 5:
                        await EliminarAsync();
                        break;
                    case 6:
                        return;
                }
            }
        }

        private async Task AgregarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Catálogo / Agregar producto");

            var nombre = Consola.LeerTexto("   Nombre: ", true, 100, null, true);
            if (nombre == null)
            {
                return;
            }

            var precio = Consola.LeerDecimal("   Precio (0 para cancelar): ", 0.01m, 99999999m, null, true);
            if (!precio.HasValue)
            {
                return;
            }

            try
            {
                var respuesta = await _controlador.CrearAsync(new Producto
                {
                    Nombre = nombre,
                    Precio = precio.Value
                });

                MostrarRespuesta(respuesta, "Producto guardado exitosamente.");
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private async Task ListarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Catálogo / Catálogo completo");

            try
            {
                var productos = await _controlador.ListarAsync();
                if (productos == null || productos.Count == 0)
                {
                    Consola.Info("No hay productos registrados en el catálogo.");
                }
                else
                {
                    var tabla = new TablaConsola("Código", "Nombre", "Precio");
                    foreach (var producto in productos)
                    {
                        tabla.Agregar(producto.NumInventario, producto.Nombre, Consola.Moneda(producto.Precio));
                    }

                    tabla.Imprimir();
                    Consola.Info("Total de productos: " + productos.Count);
                }
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private async Task BuscarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Catálogo / Buscar producto");

            var codigo = Consola.LeerEntero("   Código del producto (0 para volver): ", 1, int.MaxValue, true);
            if (!codigo.HasValue)
            {
                return;
            }

            try
            {
                var producto = await _controlador.BuscarAsync(codigo.Value);
                MostrarProducto(producto);
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private async Task ActualizarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Catálogo / Actualizar datos de producto");

            var codigo = Consola.LeerEntero("   Código del producto (0 para volver): ", 1, int.MaxValue, true);
            if (!codigo.HasValue)
            {
                return;
            }

            Producto producto;
            try
            {
                producto = await _controlador.BuscarAsync(codigo.Value);
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
                Consola.Pausa();
                return;
            }

            Consola.Info("Datos actuales:");
            MostrarProducto(producto);

            var nombre = Consola.LeerTexto(
                "   Nuevo nombre [" + producto.Nombre + "]: ",
                true,
                100,
                producto.Nombre,
                false);

            var precio = Consola.LeerDecimal(
                "   Nuevo precio [" + Consola.Moneda(producto.Precio) + "]: ",
                0.01m,
                99999999m,
                producto.Precio,
                false);

            producto.Nombre = nombre;
            producto.Precio = precio.Value;

            try
            {
                var respuesta = await _controlador.ActualizarAsync(producto);
                MostrarRespuesta(respuesta, "Datos actualizados exitosamente.");
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private async Task EliminarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Catálogo / Eliminar producto");

            var codigo = Consola.LeerEntero("   Código del producto (0 para volver): ", 1, int.MaxValue, true);
            if (!codigo.HasValue)
            {
                return;
            }

            Producto producto;
            try
            {
                producto = await _controlador.BuscarAsync(codigo.Value);
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
                Consola.Pausa();
                return;
            }

            MostrarProducto(producto);

            if (!Consola.Confirmar("   ¿Desea eliminar el producto " + producto.Nombre + "?"))
            {
                Consola.Info("Operación cancelada.");
                Consola.Pausa();
                return;
            }

            try
            {
                await _controlador.EliminarAsync(codigo.Value);
                Consola.Exito("Producto eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private static void MostrarProducto(Producto producto)
        {
            if (producto == null)
            {
                Consola.Aviso("No se recibieron datos del producto.");
                return;
            }

            var tabla = new TablaConsola("Código", "Nombre", "Precio");
            tabla.Agregar(producto.NumInventario, producto.Nombre, Consola.Moneda(producto.Precio));
            tabla.Imprimir();
        }

        private static void MostrarRespuesta(RespuestaApi respuesta, string mensajePorDefecto)
        {
            if (respuesta != null && respuesta.Exitoso)
            {
                Consola.Exito(string.IsNullOrWhiteSpace(respuesta.Mensaje)
                    ? mensajePorDefecto
                    : respuesta.Mensaje);
                return;
            }

            Consola.Error(respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Mensaje)
                ? respuesta.Mensaje
                : mensajePorDefecto);
        }
    }
}
