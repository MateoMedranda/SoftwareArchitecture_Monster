using System;
using System.Threading.Tasks;
using CLICON_Facturacion.Controladores;
using CLICON_Facturacion.Modelos;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Vistas de consulta y mantenimiento de clientes.
    /// </summary>
    public sealed class ClientesVista
    {
        private readonly ClienteControlador _controlador;

        public ClientesVista()
        {
            _controlador = new ClienteControlador();
        }

        public async Task MostrarAsync()
        {
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Clientes");
                Consola.MenuOpcion(1, "Agregar cliente");
                Consola.MenuOpcion(2, "Ver lista de clientes");
                Consola.MenuOpcion(3, "Buscar cliente");
                Consola.MenuOpcion(4, "Actualizar datos de cliente");
                Consola.MenuOpcion(5, "Eliminar cliente");
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
            Consola.Encabezado("Clientes / Agregar cliente");

            var cedula = LeerCedula("   Cédula: ", null, true);
            if (cedula == null)
            {
                return;
            }

            var nombre = Consola.LeerTexto("   Nombre: ", true, 100, null, true);
            if (nombre == null)
            {
                return;
            }

            try
            {
                var respuesta = await _controlador.CrearAsync(new Cliente
                {
                    Cedula = cedula,
                    Nombre = nombre
                });

                MostrarRespuesta(respuesta, "Cliente guardado exitosamente.");
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
            Consola.Encabezado("Clientes / Lista de clientes");

            try
            {
                var clientes = await _controlador.ListarAsync();
                if (clientes == null || clientes.Count == 0)
                {
                    Consola.Info("No hay clientes registrados.");
                }
                else
                {
                    var tabla = new TablaConsola("Código", "Cédula", "Nombre");
                    foreach (var cliente in clientes)
                    {
                        tabla.Agregar(cliente.CodCliente, cliente.Cedula, cliente.Nombre);
                    }

                    tabla.Imprimir();
                    Consola.Info("Total de clientes: " + clientes.Count);
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
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Clientes / Buscar cliente");
                Consola.MenuOpcion(1, "Por cédula");
                Consola.MenuOpcion(2, "Por código");
                Consola.MenuOpcion(3, "Atrás");

                var opcion = Consola.LeerOpcion("   Seleccione una opción: ", 1, 3);
                if (opcion == 3)
                {
                    return;
                }

                if (opcion == 1)
                {
                    var cedula = LeerCedula("   Cédula: ", null, true);
                    if (cedula == null)
                    {
                        continue;
                    }

                    try
                    {
                        var cliente = await _controlador.BuscarPorCedulaAsync(cedula);
                        MostrarCliente(cliente);
                    }
                    catch (Exception ex)
                    {
                        Consola.MostrarError(ex);
                    }
                }
                else
                {
                    var codigo = Consola.LeerEntero("   Código (0 para volver): ", 1, int.MaxValue, true);
                    if (!codigo.HasValue)
                    {
                        continue;
                    }

                    try
                    {
                        var cliente = await _controlador.BuscarAsync(codigo.Value);
                        MostrarCliente(cliente);
                    }
                    catch (Exception ex)
                    {
                        Consola.MostrarError(ex);
                    }
                }

                Consola.Pausa();
            }
        }

        private async Task ActualizarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Clientes / Actualizar datos de cliente");

            var codigo = Consola.LeerEntero("   Código del cliente (0 para volver): ", 1, int.MaxValue, true);
            if (!codigo.HasValue)
            {
                return;
            }

            Cliente cliente;
            try
            {
                cliente = await _controlador.BuscarAsync(codigo.Value);
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
                Consola.Pausa();
                return;
            }

            Consola.Info("Datos actuales:");
            MostrarCliente(cliente);

            var cedula = LeerCedula("   Nueva cédula [" + cliente.Cedula + "]: ", cliente.Cedula, false);
            var nombre = Consola.LeerTexto("   Nuevo nombre [" + cliente.Nombre + "]: ", true, 100, cliente.Nombre, false);

            cliente.Cedula = cedula;
            cliente.Nombre = nombre;

            try
            {
                var respuesta = await _controlador.ActualizarAsync(cliente);
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
            Consola.Encabezado("Clientes / Eliminar cliente");

            var codigo = Consola.LeerEntero("   Código del cliente (0 para volver): ", 1, int.MaxValue, true);
            if (!codigo.HasValue)
            {
                return;
            }

            Cliente cliente;
            try
            {
                cliente = await _controlador.BuscarAsync(codigo.Value);
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
                Consola.Pausa();
                return;
            }

            MostrarCliente(cliente);

            if (!Consola.Confirmar("   ¿Desea eliminar el cliente " + cliente.Nombre + "?"))
            {
                Consola.Info("Operación cancelada.");
                Consola.Pausa();
                return;
            }

            try
            {
                await _controlador.EliminarAsync(codigo.Value);
                Consola.Exito("Cliente eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private static string LeerCedula(string etiqueta, string valorActual, bool permitirCancelar)
        {
            while (true)
            {
                var valor = Consola.LeerTexto(etiqueta, true, 10, valorActual, permitirCancelar);
                if (valor == null)
                {
                    return null;
                }

                if (EsCedulaValida(valor))
                {
                    return valor;
                }

                Consola.Error("La cédula debe tener exactamente 10 dígitos.");
            }
        }

        private static bool EsCedulaValida(string valor)
        {
            if (string.IsNullOrEmpty(valor) || valor.Length != 10)
            {
                return false;
            }

            foreach (var caracter in valor)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        private static void MostrarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                Consola.Aviso("No se recibieron datos del cliente.");
                return;
            }

            var tabla = new TablaConsola("Código", "Cédula", "Nombre");
            tabla.Agregar(cliente.CodCliente, cliente.Cedula, cliente.Nombre);
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
