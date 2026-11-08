namespace CLI_WEB_RESTFUL_DOTNET.Modelos.Utilidades
{
    public static class CargadorEnv
    {
        private const string NombreArchivo = ".development.env";

        /// <summary>
        /// Busca el archivo a partir de la carpeta indicada (y sus carpetas padre)
        /// y devuelve sus variables como pares clave/valor.
        /// </summary>
        public static Dictionary<string, string?> Cargar(string rutaInicio)
        {
            var variables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var rutaArchivo = BuscarArchivo(rutaInicio);

            if (rutaArchivo is null)
            {
                Console.WriteLine($"[Advertencia] No se encontró el archivo '{NombreArchivo}'.");
                return variables;
            }

            Console.WriteLine($"[Configuración] Variables cargadas desde '{rutaArchivo}'.");

            foreach (var linea in File.ReadAllLines(rutaArchivo))
            {
                var texto = linea.Trim();
                if (texto.Length == 0 || texto.StartsWith('#'))
                {
                    continue;
                }

                var separador = texto.IndexOf('=');
                if (separador <= 0)
                {
                    continue;
                }

                var clave = texto[..separador].Trim();
                var valor = texto[(separador + 1)..].Trim();
                variables[clave] = valor;
            }

            return variables;
        }

        private static string? BuscarArchivo(string rutaInicio)
        {
            var directorio = new DirectoryInfo(rutaInicio);

            while (directorio is not null)
            {
                var rutaCandidata = Path.Combine(directorio.FullName, NombreArchivo);
                if (File.Exists(rutaCandidata))
                {
                    return rutaCandidata;
                }

                directorio = directorio.Parent;
            }

            return null;
        }
    }
}
