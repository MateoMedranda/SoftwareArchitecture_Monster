using System;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CLICON_Facturacion.Servicios;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Utilidades comunes para las vistas de consola: colores, entradas
    /// validadas, mensajes y formato de datos.
    /// </summary>
    public static class Consola
    {
        public const string HexCelulares = "#74E0F2";
        public const string HexMonster = "#C674F2";

        private const string HexExito = "#4ADE80";
        private const string HexError = "#F87171";
        private const string HexAviso = "#FBBF24";
        private const string HexTenue = "#9CA3AF";

        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 0x0004;

        private static bool _ansiDisponible;
        private static ConsoleColor _colorOriginal;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static int Ancho
        {
            get
            {
                try
                {
                    return Math.Max(40, Console.WindowWidth);
                }
                catch (Exception)
                {
                    return 80;
                }
            }
        }

        public static void Inicializar()
        {
            try
            {
                _colorOriginal = Console.ForegroundColor;
            }
            catch (Exception)
            {
                _colorOriginal = ConsoleColor.Gray;
            }

            try
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }
            catch (Exception)
            {
                // Algunas consolas redirigidas no permiten cambiar la codificación.
            }

            try
            {
                Console.Title = "Celulares Monster | Facturación";
            }
            catch (Exception)
            {
                // El título no es indispensable para el funcionamiento.
            }

            _ansiDisponible = HabilitarAnsi();
        }

        public static void Restaurar()
        {
            try
            {
                if (_ansiDisponible)
                {
                    Console.Write("\u001b[0m");
                }

                Console.ForegroundColor = _colorOriginal;
                Console.ResetColor();
            }
            catch (Exception)
            {
                // No se puede hacer nada si la consola ya no está disponible.
            }
        }

        public static void Limpiar()
        {
            try
            {
                Console.Clear();
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
        }

        public static void Encabezado(string seccion)
        {
            Console.WriteLine();
            EscribirLinea("   CELULARES MONSTER", ConsoleColor.Cyan, HexCelulares);
            EscribirLinea("   " + (seccion ?? string.Empty), ConsoleColor.White);
            EscribirLinea("   " + new string('-', Math.Min(56, Math.Max(10, Ancho - 6))), ConsoleColor.DarkGray, HexTenue);
        }

        public static void MenuOpcion(int numero, string texto)
        {
            EscribirLinea("   [" + numero + "] " + texto, ConsoleColor.White);
        }

        public static void Exito(string mensaje)
        {
            EscribirLinea("   " + mensaje, ConsoleColor.Green, HexExito);
        }

        public static void Error(string mensaje)
        {
            EscribirLinea("   " + mensaje, ConsoleColor.Red, HexError);
        }

        public static void Aviso(string mensaje)
        {
            EscribirLinea("   " + mensaje, ConsoleColor.Yellow, HexAviso);
        }

        public static void Info(string mensaje)
        {
            EscribirLinea("   " + mensaje, ConsoleColor.Cyan, HexCelulares);
        }

        public static void Tenue(string mensaje)
        {
            EscribirLinea("   " + mensaje, ConsoleColor.Gray, HexTenue);
        }

        public static void MostrarError(Exception ex)
        {
            Error("[Error] " + MensajeDeError(ex));
        }

        public static string MensajeDeError(Exception ex)
        {
            var api = ex as ApiException;
            if (api != null)
            {
                if (!string.IsNullOrWhiteSpace(api.Message))
                {
                    return api.Message;
                }

                if (api.StatusCode == 404)
                {
                    return "El recurso solicitado no existe.";
                }

                if (api.StatusCode == 409)
                {
                    return "La operación entra en conflicto con los datos actuales.";
                }

                return "El API respondió con el código " + api.StatusCode + ".";
            }

            if (ex is HttpRequestException)
            {
                return "No se pudo conectar con el API de facturación. Verifique que el servidor esté en ejecución.";
            }

            if (ex is TaskCanceledException)
            {
                return "La solicitud al API excedió el tiempo de espera.";
            }

            if (ex is SerializationException)
            {
                return "El API devolvió una respuesta con un formato inesperado.";
            }

            return string.IsNullOrWhiteSpace(ex.Message)
                ? "Ocurrió un error inesperado."
                : ex.Message;
        }

        public static void Pausa(string mensaje = "   Presione Enter para continuar...")
        {
            Console.WriteLine();
            Console.Write(mensaje);
            Console.ReadLine();
        }

        public static string Centrar(string texto)
        {
            texto = texto ?? string.Empty;
            var ancho = Ancho;
            if (texto.Length >= ancho)
            {
                return texto;
            }

            return new string(' ', (ancho - texto.Length) / 2) + texto;
        }

        public static int LeerOpcion(string etiqueta, int minimo, int maximo)
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write(etiqueta);
                var linea = Console.ReadLine();
                if (linea == null)
                {
                    return minimo;
                }

                int opcion;
                if (int.TryParse(linea.Trim(), out opcion) && opcion >= minimo && opcion <= maximo)
                {
                    return opcion;
                }

                Error("Ingrese una opción entre " + minimo + " y " + maximo + ".");
            }
        }

        public static string LeerTexto(
            string etiqueta,
            bool obligatorio,
            int maxLongitud,
            string valorActual = null,
            bool permitirCancelar = false)
        {
            while (true)
            {
                Console.Write(etiqueta);
                var linea = Console.ReadLine();
                if (linea == null)
                {
                    return valorActual;
                }

                linea = linea.Trim();

                if (linea.Length == 0)
                {
                    if (valorActual != null)
                    {
                        return valorActual;
                    }

                    if (permitirCancelar || !obligatorio)
                    {
                        return null;
                    }

                    Error("Este campo es obligatorio.");
                    continue;
                }

                if (maxLongitud > 0 && linea.Length > maxLongitud)
                {
                    Error("El valor no puede superar los " + maxLongitud + " caracteres.");
                    continue;
                }

                return linea;
            }
        }

        public static int? LeerEntero(
            string etiqueta,
            int minimo,
            int maximo,
            bool ceroParaVolver)
        {
            while (true)
            {
                Console.Write(etiqueta);
                var linea = Console.ReadLine();
                if (linea == null)
                {
                    return null;
                }

                linea = linea.Trim();
                if (linea.Length == 0)
                {
                    return null;
                }

                int valor;
                if (!int.TryParse(linea, out valor))
                {
                    Error("Ingrese un número entero válido.");
                    continue;
                }

                if (ceroParaVolver && valor == 0)
                {
                    return null;
                }

                if (valor < minimo || valor > maximo)
                {
                    Error("Ingrese un valor entre " + minimo + " y " + maximo + ".");
                    continue;
                }

                return valor;
            }
        }

        public static decimal? LeerDecimal(
            string etiqueta,
            decimal minimo,
            decimal maximo,
            decimal? valorActual = null,
            bool ceroParaVolver = false)
        {
            while (true)
            {
                Console.Write(etiqueta);
                var linea = Console.ReadLine();
                if (linea == null)
                {
                    return valorActual;
                }

                linea = linea.Trim();
                if (linea.Length == 0)
                {
                    if (valorActual.HasValue)
                    {
                        return valorActual;
                    }

                    if (ceroParaVolver)
                    {
                        return null;
                    }

                    Error("Este campo es obligatorio.");
                    continue;
                }

                decimal valor;
                if (!TryParseDecimal(linea, out valor))
                {
                    Error("Ingrese un valor numérico válido.");
                    continue;
                }

                if (ceroParaVolver && valor == 0m)
                {
                    return null;
                }

                if (valor < minimo || valor > maximo)
                {
                    Error("Ingrese un valor entre " + Moneda(minimo) + " y " + Moneda(maximo) + ".");
                    continue;
                }

                return valor;
            }
        }

        public static bool Confirmar(string pregunta)
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write(pregunta + " [S/N]: ");
                var linea = Console.ReadLine();
                if (linea == null)
                {
                    return false;
                }

                linea = linea.Trim().ToLowerInvariant();
                if (linea == "s" || linea == "si" || linea == "sí")
                {
                    return true;
                }

                if (linea.Length == 0 || linea == "n" || linea == "no")
                {
                    return false;
                }

                Error("Responda S o N.");
            }
        }

        public static string Moneda(decimal valor)
        {
            return "$" + valor.ToString("N2", CultureInfo.InvariantCulture);
        }

        public static string Fecha(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            DateTime fecha;
            if (DateTime.TryParse(valor, CultureInfo.CurrentCulture, DateTimeStyles.None, out fecha) ||
                DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out fecha))
            {
                return fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            return valor;
        }

        public static string SiNo(bool valor)
        {
            return valor ? "Sí" : "No";
        }

        public static void Escribir(string texto, ConsoleColor colorRespaldo, string hex = null)
        {
            int rojo;
            int verde;
            int azul;
            if (_ansiDisponible && TryHex(hex, out rojo, out verde, out azul))
            {
                Console.Write("\u001b[38;2;" + rojo + ";" + verde + ";" + azul + "m" + texto + "\u001b[0m");
                return;
            }

            var anterior = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = colorRespaldo;
                Console.Write(texto);
            }
            finally
            {
                Console.ForegroundColor = anterior;
            }
        }

        public static void EscribirLinea(string texto, ConsoleColor colorRespaldo, string hex = null)
        {
            Escribir(texto, colorRespaldo, hex);
            Console.WriteLine();
        }

        private static bool TryParseDecimal(string texto, out decimal valor)
        {
            valor = 0m;
            texto = (texto ?? string.Empty)
                .Replace("$", string.Empty)
                .Replace(" ", string.Empty)
                .Trim();

            if (texto.Length == 0)
            {
                return false;
            }

            var ultimaComa = texto.LastIndexOf(',');
            var ultimoPunto = texto.LastIndexOf('.');
            string normalizado;

            if (ultimaComa >= 0 && ultimoPunto >= 0)
            {
                normalizado = ultimaComa > ultimoPunto
                    ? texto.Replace(".", string.Empty).Replace(',', '.')
                    : texto.Replace(",", string.Empty);
            }
            else if (ultimaComa >= 0)
            {
                normalizado = texto.Replace(',', '.');
            }
            else
            {
                normalizado = texto;
            }

            return decimal.TryParse(
                normalizado,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out valor);
        }

        private static bool HabilitarAnsi()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                return true;
            }

            try
            {
                var handle = GetStdHandle(StdOutputHandle);
                uint modo;
                if (!GetConsoleMode(handle, out modo))
                {
                    return false;
                }

                return SetConsoleMode(handle, modo | EnableVirtualTerminalProcessing);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryHex(string hex, out int rojo, out int verde, out int azul)
        {
            rojo = 0;
            verde = 0;
            azul = 0;

            if (string.IsNullOrWhiteSpace(hex))
            {
                return false;
            }

            hex = hex.Trim().TrimStart('#');
            if (hex.Length != 6)
            {
                return false;
            }

            return int.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out rojo) &&
                   int.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out verde) &&
                   int.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out azul);
        }
    }
}
