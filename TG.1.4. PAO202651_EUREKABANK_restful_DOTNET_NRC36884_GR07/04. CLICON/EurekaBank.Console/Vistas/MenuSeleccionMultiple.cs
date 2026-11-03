namespace EurekaBank.Console.Vistas;

/// <summary>Selector: ↑/↓ para desplazarse, Enter para elegir y Esc para volver.</summary>
public sealed class MenuSeleccionMultiple(string titulo, IReadOnlyList<string> opciones)
{
    public int Mostrar()
    {
        var seleccion = 0;
        var inicio = System.Console.CursorTop;
        var ancho = Math.Max(titulo.Length, opciones.Max(x => x.Length + 4));
        var texto = System.Console.ForegroundColor;
        var fondo = System.Console.BackgroundColor;

        try
        {
            System.Console.CursorVisible = false;
            while (true)
            {
                System.Console.SetCursorPosition(0, inicio);
                Pintar(ConsoleColor.Cyan, titulo, ancho);
                Pintar(ConsoleColor.DarkCyan, new string('─', ancho), ancho);
                for (var i = 0; i < opciones.Count; i++)
                {
                    var activa = i == seleccion;
                    System.Console.BackgroundColor = activa ? ConsoleColor.DarkCyan : fondo;
                    System.Console.ForegroundColor = activa ? ConsoleColor.White : texto;
                    System.Console.WriteLine(((activa ? "▶ " : "  ") + opciones[i]).PadRight(ancho));
                }
                System.Console.BackgroundColor = fondo;
                Pintar(ConsoleColor.DarkGray, "↑/↓ navegar · Enter seleccionar · Esc volver", ancho);
                System.Console.ForegroundColor = texto;

                switch (System.Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.UpArrow: seleccion = (seleccion + opciones.Count - 1) % opciones.Count; break;
                    case ConsoleKey.DownArrow: seleccion = (seleccion + 1) % opciones.Count; break;
                    case ConsoleKey.Enter: return seleccion;
                    case ConsoleKey.Escape: return -1;
                }
            }
        }
        finally
        {
            System.Console.CursorVisible = true;
            System.Console.ResetColor();
        }
    }

    private static void Pintar(ConsoleColor color, string valor, int ancho) { System.Console.ForegroundColor = color; System.Console.WriteLine(valor.PadRight(ancho)); }
}
