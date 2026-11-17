using System.Globalization;
using EurekaBank.Contracts;

namespace EurekaBank.Console.Vistas;

public static class PresentacionConsola
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-EC");

    public static void MostrarMarca()
    {
        Color(ConsoleColor.Cyan, "╔══════════════════════════════════════════════════╗");
        Color(ConsoleColor.Cyan, "║                 E U R E K A B A N K              ║");
        Color(ConsoleColor.DarkCyan, "║                  BANCA DIGITAL                   ║");
        Color(ConsoleColor.Cyan, "╚══════════════════════════════════════════════════╝");
        System.Console.WriteLine();
    }

    public static void MostrarTitulo(string titulo) { Color(ConsoleColor.Yellow, $"▌ {titulo.ToUpperInvariant()}"); System.Console.WriteLine(); }
    public static void Info(string mensaje) => Color(ConsoleColor.Cyan, mensaje);
    public static void Error(string mensaje) => Color(ConsoleColor.Red, $"[ERROR] {mensaje}");

    public static bool Resultado<T>(ServiceResult<T> resultado)
    {
        if (!resultado.Ok) { Error(resultado.Message); return false; }
        Color(ConsoleColor.Green, $"[OK] {resultado.Message}");
        return true;
    }

    public static string PedirTexto(string etiqueta)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write($"{etiqueta}: ");
        System.Console.ResetColor();
        return (System.Console.ReadLine() ?? string.Empty).Trim();
    }

    public static string PedirContrasena(string etiqueta)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write($"{etiqueta}: ");
        System.Console.ResetColor();
        var valor = new System.Text.StringBuilder();
        while (true)
        {
            var tecla = System.Console.ReadKey(intercept: true);
            if (tecla.Key == ConsoleKey.Enter) { System.Console.WriteLine(); return valor.ToString(); }
            if (tecla.Key == ConsoleKey.Backspace && valor.Length > 0) { valor.Length--; System.Console.Write("\b \b"); }
            else if (!char.IsControl(tecla.KeyChar)) { valor.Append(tecla.KeyChar); System.Console.Write('•'); }
        }
    }

    public static bool PedirMonto(string etiqueta, out decimal monto)
    {
        var texto = PedirTexto(etiqueta);
        if ((decimal.TryParse(texto, NumberStyles.Number, Cultura, out monto) || decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out monto)) && monto > 0) return true;
        Error("Ingrese un monto numérico mayor que cero.");
        return false;
    }

    public static string Dinero(decimal monto) => $"USD {monto:N2}";

    public static void Tabla(IReadOnlyList<string> cabeceras, IEnumerable<string[]> filas)
    {
        var datos = filas.ToList();
        var anchos = cabeceras.Select(x => x.Length).ToArray();
        foreach (var fila in datos) for (var i = 0; i < Math.Min(fila.Length, anchos.Length); i++) anchos[i] = Math.Max(anchos[i], fila[i].Length);
        var borde = "+" + string.Join("+", anchos.Select(x => new string('-', x + 2))) + "+";
        Color(ConsoleColor.DarkCyan, borde);
        System.Console.ForegroundColor = ConsoleColor.Cyan; Fila(cabeceras, anchos); System.Console.ResetColor();
        Color(ConsoleColor.DarkCyan, borde);
        foreach (var fila in datos) Fila(fila, anchos);
        Color(ConsoleColor.DarkCyan, borde);
    }

    public static void EsperarTecla()
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.Write("Presione una tecla para continuar...");
        System.Console.ResetColor();
        System.Console.ReadKey(intercept: true);
    }

    private static void Fila(IReadOnlyList<string> valores, IReadOnlyList<int> anchos) => System.Console.WriteLine("| " + string.Join(" | ", valores.Select((x, i) => x.PadRight(anchos[i]))) + " |");
    private static void Color(ConsoleColor color, string texto) { System.Console.ForegroundColor = color; System.Console.WriteLine(texto); System.Console.ResetColor(); }
}
