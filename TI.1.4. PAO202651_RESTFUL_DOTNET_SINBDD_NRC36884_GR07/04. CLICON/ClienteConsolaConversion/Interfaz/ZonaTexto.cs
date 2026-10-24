namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Región borruble de la consola.
    /// Marca el inicio de un bloque (cabecera + selector o formulario) y permite borrar
    /// exactamente las líneas escritas desde entonces, de modo que el siguiente bloque
    /// se dibuje en el mismo sitio y no se acumule contenido anterior.
    /// Los mensajes impresos después de Borrar() quedan como registro y ya no se borran.
    /// </summary>
    public sealed class ZonaTexto
    {
        private int _izquierda;
        private int _arriba;
        private bool _activa;

        /// <summary>Marca el inicio del bloque que se va a dibujar.</summary>
        public void Comenzar()
        {
            try
            {
                _izquierda = Console.CursorLeft;
                _arriba = Console.CursorTop;
                _activa = true;
            }
            catch (IOException)
            {
                // Consola no disponible (salida redirigida): no hay zona que borrar.
                _activa = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                _activa = false;
            }
        }

        /// <summary>
        /// Borra todas las líneas escritas desde Comenzar() hasta la posición actual del cursor
        /// y devuelve el cursor al inicio del bloque. Si no hay bloque activo no hace nada.
        /// </summary>
        public void Borrar()
        {
            if (!_activa)
            {
                return;
            }

            _activa = false;

            try
            {
                int ultimaFila = Console.CursorTop;

                if (ultimaFila < _arriba || _izquierda < 0)
                {
                    return;
                }

                int ancho = Math.Max(1, AnchoConsola() - _izquierda);

                for (int fila = _arriba; fila <= ultimaFila; fila++)
                {
                    Console.SetCursorPosition(_izquierda, fila);
                    Console.Write(new string(' ', ancho));
                }

                Console.SetCursorPosition(_izquierda, _arriba);
            }
            catch (ArgumentOutOfRangeException)
            {
                // La consola cambió de tamaño durante el borrado: se deja como está.
            }
            catch (IOException)
            {
                // Consola no disponible: se deja como está.
            }
        }

        private static int AnchoConsola()
        {
            try
            {
                return Math.Max(20, Console.WindowWidth);
            }
            catch (IOException)
            {
                return 80;
            }
            catch (ArgumentOutOfRangeException)
            {
                return 80;
            }
        }
    }
}
