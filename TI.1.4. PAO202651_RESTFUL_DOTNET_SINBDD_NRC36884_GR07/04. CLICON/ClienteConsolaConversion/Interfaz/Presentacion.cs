namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Presentación general de la consola: rótulo de arranque, cabecera de pantalla,
    /// mensajes con color y espera de tecla.
    /// Los mensajes del servidor siempre se imprimen tal cual, sin reescribirlos.
    /// </summary>
    public static class Presentacion
    {
        /// <summary>Fuente de 5x5 píxeles para el rótulo de arranque (solo letras necesarias).</summary>
        private static readonly Dictionary<char, string[]> Fuente =
            new Dictionary<char, string[]>()
            {
                { 'M', new[] { "#...#", "##.##", "#.#.#", "#...#", "#...#" } },
                { 'O', new[] { ".###.", "#...#", "#...#", "#...#", ".###." } },
                { 'N', new[] { "#...#", "##..#", "#.#.#", "#..##", "#...#" } },
                { 'S', new[] { ".####", "#....", ".###.", "....#", "####." } },
                { 'T', new[] { "#####", "..#..", "..#..", "..#..", "..#.." } },
                { 'E', new[] { "#####", "#....", "####.", "#....", "#####" } },
                { 'R', new[] { "####.", "#...#", "####.", "#.#..", "#..#." } }
            };

        private const int AltoGlifo = 5;

        /// <summary>Rótulo MONSTER en caracteres de bloque. Se imprime una sola vez al arrancar.</summary>
        public static void MostrarLogo()
        {
            int escala = 1;
            try
            {
                escala = Console.WindowWidth >= 78 ? 2 : 1;
            }
            catch (IOException)
            {
                escala = 1;
            }
            catch (ArgumentOutOfRangeException)
            {
                escala = 1;
            }

            ConsoleColor colorAnterior = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (string fila in DibujarTexto("MONSTER", escala))
            {
                Console.WriteLine(fila);
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  Conversión de Unidades · Cliente RESTful");
            Console.ForegroundColor = colorAnterior;
            Console.WriteLine();
        }

        /// <summary>Cabecera corta que encabeza cada pantalla/etapa.</summary>
        public static void MostrarCabecera()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("MONSTER · Conversión de Unidades");
            Console.ResetColor();
        }

        /// <summary>Muestra el mensaje de error del servidor exactamente como llegó, con el prefijo [ERROR] en rojo.</summary>
        public static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + mensaje);
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] " + mensaje);
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void MostrarInfo(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(mensaje);
            Console.ResetColor();
        }

        /// <summary>Pide una línea de texto visible (por ejemplo el usuario).</summary>
        public static string PedirTexto(string etiqueta)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(etiqueta);
            Console.ResetColor();
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        public static void EsperarTecla()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>Convierte un texto en líneas de caracteres de bloque usando la fuente interna.</summary>
        private static IEnumerable<string> DibujarTexto(string texto, int escala)
        {
            var lineas = new string[AltoGlifo];
            bool primero = true;

            foreach (char caracter in texto.ToUpperInvariant())
            {
                if (!Fuente.TryGetValue(caracter, out string[]? glifo) || glifo.Length != AltoGlifo)
                {
                    continue;
                }

                for (int fila = 0; fila < AltoGlifo; fila++)
                {
                    string pixeles = Expandir(glifo[fila], escala);
                    lineas[fila] = primero ? pixeles : lineas[fila] + " " + pixeles;
                }

                primero = false;
            }

            return lineas;
        }

        private static string Expandir(string fila, int escala)
        {
            var construida = new System.Text.StringBuilder(fila.Length * escala);

            foreach (char pixel in fila)
            {
                construida.Append(pixel == '#' ? new string('█', escala) : new string(' ', escala));
            }

            return construida.ToString();
        }
    }
}
