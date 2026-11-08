using System.Globalization;
using System.Text;
using ClienteConsolaConversion.Cliente;
using ClienteConsolaConversion.Configuracion;
using ClienteConsolaConversion.Estado;
using ClienteConsolaConversion.Interfaz;
using ClienteConsolaConversion.Servicio;

namespace ClienteConsolaConversion
{
    /// <summary>
    /// Cliente de consola del servicio SOAP de conversión de unidades.
    /// Flujo: login -> menú principal -> convertir / cerrar sesión / salir.
    /// Cada bloque (cabecera + selector o formulario) ocupa una "zona" que se borra
    /// antes de dibujar el siguiente, sin usar Console.Clear().
    /// Esc retrocede una etapa; Esc en el menú principal no hace nada.
    /// </summary>
    internal static class Program
    {
        /// <summary>Zona del bloque activo: cabecera + selector o formulario en pantalla.</summary>
        private static readonly ZonaTexto Zona = new ZonaTexto();

        private static async Task Main()
        {
            ConfigurarConsola();

            ConfiguracionCliente configuracion = ConfiguracionCliente.Cargar();
            var servicio = new ClienteSoapConversion(configuracion.UrlEndpoint);
            var sesion = new SesionActual();

            Presentacion.MostrarLogo();

            while (true)
            {
                while (!sesion.Autenticado)
                {
                    await PantallaInicioSesion(servicio, sesion);
                }

                int opcion = MenuPrincipal();

                if (opcion == 0)
                {
                    await FlujoConversion(servicio, sesion);
                }
                else if (opcion == 1)
                {
                    Zona.Borrar();
                    sesion.Cerrar();
                    Presentacion.MostrarInfo("Sesión cerrada. Inicie sesión nuevamente.");
                    Console.WriteLine();
                }
                else
                {
                    Zona.Borrar();
                    break;
                }
            }

            Presentacion.MostrarInfo("Hasta luego.");
        }

        private static async Task PantallaInicioSesion(ClienteSoapConversion servicio, SesionActual sesion)
        {
            // Borra el intento anterior (formulario + mensaje de error) y redibuja en su lugar.
            Zona.Borrar();
            Zona.Comenzar();
            Presentacion.MostrarCabecera();
            Presentacion.MostrarInfo("Inicio de sesión");

            string usuario = Presentacion.PedirTexto("Nombre de usuario: ");
            if (string.IsNullOrWhiteSpace(usuario))
            {
                Presentacion.MostrarError("El usuario no puede estar vacío.");
                Presentacion.EsperarTecla();
                return;
            }

            string contrasena = EntradaContrasena.Leer("Contraseña: ");
            if (string.IsNullOrEmpty(contrasena))
            {
                Presentacion.MostrarError("La contraseña no puede estar vacía.");
                Presentacion.EsperarTecla();
                return;
            }

            RespuestaInicioSesion respuesta = await servicio.IniciarSesionAsync(usuario, contrasena);

            if (respuesta.Exito && !string.IsNullOrEmpty(respuesta.Token))
            {
                // Se borra solo el formulario; el mensaje del servidor queda como registro.
                Zona.Borrar();
                sesion.Iniciar(usuario, respuesta.Token);
                Presentacion.MostrarExito(respuesta.Mensaje);
            }
            else
            {
                Presentacion.MostrarError(respuesta.Mensaje);
                Presentacion.EsperarTecla();
            }
        }

        /// <summary>Devuelve 0 = Convertir, 1 = Cerrar sesión, 2 = Salir.</summary>
        private static int MenuPrincipal()
        {
            while (true)
            {
                Zona.Borrar();
                Zona.Comenzar();
                Presentacion.MostrarCabecera();

                var menu = new MenuSeleccionMultiple(
                    "Menú principal",
                    new[] { "Convertir", "Cerrar sesión", "Salir" });

                int opcion = menu.Mostrar();

                if (opcion < 0)
                {
                    // Esc en el menú principal no hace nada: se borra y se vuelve a dibujar.
                    continue;
                }

                return opcion;
            }
        }

        private static async Task FlujoConversion(ClienteSoapConversion servicio, SesionActual sesion)
        {
            // Etapas: 0 = categoría, 1 = unidad de origen, 2 = unidad de destino, 3 = valor.
            int etapa = 0;
            var categoria = CategoriaUnidad.Temperatura;
            IReadOnlyList<string> unidades = Array.Empty<string>();
            int indiceOrigen = -1;
            int indiceDestino = -1;

            while (true)
            {
                // Borra el bloque de la etapa anterior (o el menú principal) y redibuja.
                Zona.Borrar();
                Zona.Comenzar();
                Presentacion.MostrarCabecera();

                if (etapa == 0)
                {
                    var menu = new MenuSeleccionMultiple(
                        "Categoría de conversión",
                        CatalogoUnidades.Categorias());

                    int seleccion = menu.Mostrar();
                    if (seleccion < 0)
                    {
                        // Esc: se vuelve al menú principal (este bloque se borra allí).
                        return;
                    }

                    categoria = (CategoriaUnidad)seleccion;
                    unidades = CatalogoUnidades.Obtener(categoria);
                    etapa = 1;
                    continue;
                }

                if (etapa == 1)
                {
                    var menu = new MenuSeleccionMultiple(
                        $"Origen · {categoria}",
                        unidades);

                    int seleccion = menu.Mostrar();
                    if (seleccion < 0)
                    {
                        etapa = 0; // Esc: volver a elegir categoría.
                        continue;
                    }

                    indiceOrigen = seleccion;
                    etapa = 2;
                    continue;
                }

                if (etapa == 2)
                {
                    var menu = new MenuSeleccionMultiple(
                        $"Destino · {categoria} (origen: {unidades[indiceOrigen]})",
                        unidades);

                    int seleccion = menu.Mostrar();
                    if (seleccion < 0)
                    {
                        etapa = 1; // Esc: volver a elegir origen.
                        continue;
                    }

                    if (seleccion == indiceOrigen)
                    {
                        Zona.Borrar();
                        Presentacion.MostrarError("No es posible convertir a la misma unidad.");
                        Presentacion.EsperarTecla();
                        continue; // Se vuelve a elegir destino (el mensaje queda como registro).
                    }

                    indiceDestino = seleccion;
                    etapa = 3;
                    continue;
                }

                // Etapa 3: lectura del valor y llamada al servicio.
                string etiqueta = string.Format(
                    CultureInfo.InvariantCulture,
                    "Valor a convertir ({0} → {1}): ",
                    unidades[indiceOrigen],
                    unidades[indiceDestino]);

                if (!EntradaNumerica.Leer(etiqueta, out double valor))
                {
                    return;
                }

                RespuestaConversion respuesta = await servicio.ConvertirAsync(
                    sesion.Token!,
                    categoria,
                    unidades[indiceOrigen],
                    unidades[indiceDestino],
                    valor);

                // Se borra la pantalla de la etapa; el resultado queda como registro.
                Zona.Borrar();

                if (respuesta.Exito)
                {
                    Presentacion.MostrarExito(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} {1} = {2:0.0000} {3}",
                        valor.ToString("0.####", CultureInfo.InvariantCulture),
                        unidades[indiceOrigen],
                        respuesta.ValorConvertido,
                        unidades[indiceDestino]));
                }
                else
                {
                    Presentacion.MostrarError(respuesta.Mensaje);
                }

                Presentacion.EsperarTecla();
                return;
            }
        }

        private static void ConfigurarConsola()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch (IOException)
            {
                // Consola sin soporte de codificación: se mantiene la predeterminada.
            }

            Console.Title = "Monster - Conversión de Unidades (Cliente SOAP)";
        }
    }
}
