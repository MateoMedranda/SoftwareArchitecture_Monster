using System;
using System.Collections.Generic;
using System.IO;

namespace CLICON_Facturacion.Config
{
    /// <summary>
    /// Lee la configuracion local desde un archivo .env.
    /// Si el archivo no existe o una variable no esta definida, se usan
    /// los valores por defecto que apuntan al API de facturacion local.
    /// </summary>
    public static class AppConfig
    {
        public const string DefaultApiBaseUrl = "http://localhost:5226/";
        public const int DefaultTimeoutSeconds = 10;

        public static string ApiBaseUrl { get; private set; }
        public static int TimeoutSeconds { get; private set; }
        public static string EnvFilePath { get; private set; }

        static AppConfig()
        {
            ApiBaseUrl = DefaultApiBaseUrl;
            TimeoutSeconds = DefaultTimeoutSeconds;
        }

        public static void Load()
        {
            ApiBaseUrl = DefaultApiBaseUrl;
            TimeoutSeconds = DefaultTimeoutSeconds;
            EnvFilePath = BuscarArchivoEnv();

            if (string.IsNullOrWhiteSpace(EnvFilePath))
            {
                return;
            }

            try
            {
                foreach (var lineaOriginal in File.ReadAllLines(EnvFilePath))
                {
                    var linea = lineaOriginal.Trim();
                    if (linea.Length == 0 || linea.StartsWith("#"))
                    {
                        continue;
                    }

                    var separador = linea.IndexOf('=');
                    if (separador <= 0)
                    {
                        continue;
                    }

                    var clave = linea.Substring(0, separador).Trim();
                    var valor = linea.Substring(separador + 1).Trim();

                    if (valor.Length >= 2 &&
                        ((valor[0] == '"' && valor[valor.Length - 1] == '"') ||
                         (valor[0] == '\'' && valor[valor.Length - 1] == '\'')))
                    {
                        valor = valor.Substring(1, valor.Length - 2);
                    }

                    if (string.Equals(clave, "FACTURACION_API_BASE_URL", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrWhiteSpace(valor))
                        {
                            ApiBaseUrl = valor.TrimEnd('/') + "/";
                        }
                    }
                    else if (string.Equals(clave, "FACTURACION_TIMEOUT_SECONDS", StringComparison.OrdinalIgnoreCase))
                    {
                        int segundos;
                        if (int.TryParse(valor, out segundos) && segundos > 0)
                        {
                            TimeoutSeconds = segundos;
                        }
                    }
                }
            }
            catch (IOException)
            {
                // Se conservan los valores por defecto.
            }
            catch (UnauthorizedAccessException)
            {
                // Se conservan los valores por defecto.
            }

            Uri uri;
            if (!Uri.TryCreate(ApiBaseUrl, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                ApiBaseUrl = DefaultApiBaseUrl;
            }
        }

        private static string BuscarArchivoEnv()
        {
            var candidatos = new List<string>();
            var directorioEjecutable = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');

            candidatos.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".development.env"));
            candidatos.Add(Path.Combine(Directory.GetCurrentDirectory(), ".development.env"));

            var directorio = new DirectoryInfo(directorioEjecutable);
            for (var nivel = 0; nivel < 6 && directorio != null; nivel++)
            {
                candidatos.Add(Path.Combine(directorio.FullName, ".development.env"));
                directorio = directorio.Parent;
            }

            foreach (var candidato in candidatos)
            {
                try
                {
                    if (File.Exists(candidato))
                    {
                        return candidato;
                    }
                }
                catch (Exception)
                {
                    // Ignora rutas invalidas y continua con la siguiente opcion.
                }
            }

            return null;
        }
    }
}
