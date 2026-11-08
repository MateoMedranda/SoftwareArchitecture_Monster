using System.Text;

namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Lectura de contraseña enmascarada: muestra '*' por carácter,
    /// permite Backspace y nunca imprime el valor real.
    /// </summary>
    public static class EntradaContrasena
    {
        public static string Leer(string etiqueta)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(etiqueta);
            Console.ResetColor();

            StringBuilder construida = new StringBuilder();

            while (true)
            {
                ConsoleKeyInfo tecla = Console.ReadKey(intercept: true);

                if (tecla.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return construida.ToString();
                }

                if (tecla.Key == ConsoleKey.Backspace)
                {
                    if (construida.Length > 0)
                    {
                        construida.Length--;
                        Console.Write("\b \b");
                    }

                    continue;
                }

                if (tecla.KeyChar != '\0' && !char.IsControl(tecla.KeyChar))
                {
                    construida.Append(tecla.KeyChar);
                    Console.Write('*');
                }
            }
        }
    }
}
