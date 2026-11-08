using System.Text;

namespace ClienteConsolaConversion.Configuracion
{
    public static class CargadorEnv
    {
        private const string NombreArchivo = ".development.env";

        public static Dictionary<string, string?> Cargar(string rutaInicio)
        {
            var variables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            string? rutaArchivo = BuscarArchivo(rutaInicio);
            if (rutaArchivo is null)
            {
                rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), NombreArchivo);
            }
            if (rutaArchivo is null || !File.Exists(rutaArchivo))
            {
                rutaArchivo = Path.Combine(AppContext.BaseDirectory, NombreArchivo);
            }

            if (!File.Exists(rutaArchivo))
            {
                return variables;
            }

            try
            {
                foreach (var linea in File.ReadAllLines(rutaArchivo, Encoding.UTF8))
                {
                    var texto = linea.Trim();
                    if (string.IsNullOrEmpty(texto) || texto.StartsWith('#'))
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
                    if ((valor.StartsWith('"') && valor.EndsWith('"')) || (valor.StartsWith('\'') && valor.EndsWith('\'')))
                    {
                        valor = valor[1..^1];
                    }
                    variables[clave] = valor;
                }
            }
            catch
            {
            }

            return variables;
        }

        private static string? BuscarArchivo(string rutaInicio)
        {
            try
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
            }
            catch
            {
            }
            return null;
        }
    }
}
