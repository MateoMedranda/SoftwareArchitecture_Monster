using System.Globalization;

namespace ClienteConsolaConversion.Interfaz
{
    /// <summary>
    /// Lectura de valores numéricos desde la consola.
    /// Acepta negativos (ej: -10, -273.15) y separador decimal '.' o ','.
    /// </summary>
    public static class EntradaNumerica
    {
        public static bool Leer(string etiqueta, out double valor)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(etiqueta);
                Console.ResetColor();

                string? texto = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    ValorInvalido();
                    continue;
                }

                string normalizado = texto.Trim().Replace(',', '.');

                if (double.TryParse(
                        normalizado,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out valor) &&
                    !double.IsNaN(valor) &&
                    !double.IsInfinity(valor))
                {
                    return true;
                }

                ValorInvalido();
            }
        }

        private static void ValorInvalido()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Valor inválido, intente de nuevo");
            Console.ResetColor();
        }
    }
}
