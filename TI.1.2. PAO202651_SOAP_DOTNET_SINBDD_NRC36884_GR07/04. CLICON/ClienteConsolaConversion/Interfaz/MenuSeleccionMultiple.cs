namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Selector de opciones con flecha movible.
    /// ↑ / ↓ mueven la flecha, Enter confirma y Esc cancela (devuelve -1).
    /// Redibuja únicamente la región del menú para evitar parpadeo.
    /// </summary>
    public class MenuSeleccionMultiple
    {
        private readonly string _titulo;
        private readonly IReadOnlyList<string> _opciones;

        private bool _dibujado;
        private int _izquierda;
        private int _arriba;

        public MenuSeleccionMultiple(string titulo, IReadOnlyList<string> opciones)
        {
            if (opciones is null || opciones.Count == 0)
            {
                throw new ArgumentException("El menú debe tener al menos una opción.", nameof(opciones));
            }

            _titulo = titulo;
            _opciones = opciones;
        }

        /// <summary>Dibuja el menú y devuelve el índice seleccionado, o -1 si se canceló con Esc.</summary>
        public int Mostrar()
        {
            int seleccionada = 0;

            try
            {
                Console.CursorVisible = false;
            }
            catch (PlatformNotSupportedException)
            {
                // Consola sin soporte de visibilidad de cursor.
            }

            try
            {
                while (true)
                {
                    Dibujar(seleccionada);

                    ConsoleKey tecla = Console.ReadKey(intercept: true).Key;

                    if (tecla == ConsoleKey.UpArrow)
                    {
                        seleccionada = (seleccionada + _opciones.Count - 1) % _opciones.Count;
                    }
                    else if (tecla == ConsoleKey.DownArrow)
                    {
                        seleccionada = (seleccionada + 1) % _opciones.Count;
                    }
                    else if (tecla == ConsoleKey.Enter)
                    {
                        return seleccionada;
                    }
                    else if (tecla == ConsoleKey.Escape)
                    {
                        return -1;
                    }
                }
            }
            finally
            {
                try
                {
                    Console.CursorVisible = true;
                }
                catch (PlatformNotSupportedException)
                {
                    // Consola sin soporte de visibilidad de cursor.
                }

                PosicionarCursorDespuesDelMenu();
            }
        }

        private void Dibujar(int seleccionada)
        {
            if (!_dibujado)
            {
                _izquierda = Console.CursorLeft;
                _arriba = Console.CursorTop;
                _dibujado = true;
            }
            else
            {
                Console.SetCursorPosition(_izquierda, _arriba);
            }

            ConsoleColor fondoAnterior = Console.BackgroundColor;
            ConsoleColor textoAnterior = Console.ForegroundColor;

            int ancho = _titulo.Length;
            foreach (string opcion in _opciones)
            {
                ancho = Math.Max(ancho, opcion.Length + 3);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            EscribirLinea(_titulo, ancho);
            EscribirLinea(new string('─', ancho), ancho);

            for (int i = 0; i < _opciones.Count; i++)
            {
                bool activa = i == seleccionada;
                Console.BackgroundColor = activa ? ConsoleColor.DarkCyan : fondoAnterior;
                Console.ForegroundColor = activa ? ConsoleColor.White : textoAnterior;

                string texto = (activa ? "► " : "  ") + _opciones[i];
                EscribirLinea(texto, ancho);
            }

            Console.BackgroundColor = fondoAnterior;
            Console.ForegroundColor = textoAnterior;
        }

        private static void EscribirLinea(string texto, int ancho)
        {
            Console.Write(texto.PadRight(ancho));
            Console.WriteLine();
        }

        private void PosicionarCursorDespuesDelMenu()
        {
            try
            {
                int fila = _arriba + _opciones.Count + 2;
                int filaMaxima = Console.BufferHeight - 1;
                Console.SetCursorPosition(_izquierda, Math.Min(fila, filaMaxima));
            }
            catch (ArgumentOutOfRangeException)
            {
                // Si la consola es muy pequeña se deja el cursor donde está.
            }
        }
    }
}
