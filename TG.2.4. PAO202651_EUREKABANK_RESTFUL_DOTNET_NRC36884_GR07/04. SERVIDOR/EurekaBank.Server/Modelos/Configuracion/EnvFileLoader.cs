namespace EurekaBank.Server;

// Carga un archivo .env de desarrollo y lo inyecta en las variables de entorno
// del proceso. Las variables ya definidas por el sistema siempre tienen prioridad.
public static class EnvFileLoader
{
    public static bool Load(string? path = null)
    {
        var candidates = new[]
        {
            path,
            Path.Combine(Directory.GetCurrentDirectory(), ".development.env"),
            Path.Combine(AppContext.BaseDirectory, ".development.env")
        };

        var file = candidates.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p));
        if (file is null) return false;

        foreach (var raw in File.ReadLines(file))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;

            var separator = line.IndexOf('=');
            if (separator <= 0) continue;

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"', '\'');

            // Precedencia: una variable real del sistema gana sobre el archivo.
            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
        }

        return true;
    }
}
