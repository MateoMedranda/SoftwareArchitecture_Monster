using System;
using System.Text;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Logotipo de la aplicación dibujado con bloques para consola.
    /// CELULARES se muestra en #74E0F2 y MONSTER en #C674F2.
    /// </summary>
    public static class LogoPixelArt
    {
        private const char Bloque = '\u2588';

        public static void Mostrar()
        {
            Console.WriteLine();
            foreach (var linea in Construir("CELULARES"))
            {
                Consola.EscribirLinea(Consola.Centrar(linea), ConsoleColor.Cyan, Consola.HexCelulares);
            }

            Console.WriteLine();
            foreach (var linea in Construir("MONSTER"))
            {
                Consola.EscribirLinea(Consola.Centrar(linea), ConsoleColor.Magenta, Consola.HexMonster);
            }

            Console.WriteLine();
            Consola.Tenue(Consola.Centrar("Sistema de facturación"));
            Console.WriteLine();
        }

        private static string[] Construir(string palabra)
        {
            var lineas = new StringBuilder[5];
            for (var i = 0; i < lineas.Length; i++)
            {
                lineas[i] = new StringBuilder();
            }

            foreach (var letra in (palabra ?? string.Empty).ToUpperInvariant())
            {
                var patron = ObtenerPatron(letra);
                for (var fila = 0; fila < 5; fila++)
                {
                    lineas[fila].Append(patron[fila]);
                    lineas[fila].Append(' ');
                }
            }

            var resultado = new string[5];
            var ancho = 0;
            for (var i = 0; i < lineas.Length; i++)
            {
                resultado[i] = lineas[i].ToString().TrimEnd();
                ancho = Math.Max(ancho, resultado[i].Length);
            }

            for (var i = 0; i < resultado.Length; i++)
            {
                resultado[i] = resultado[i].PadRight(ancho);
            }

            return resultado;
        }

        private static string[] ObtenerPatron(char letra)
        {
            switch (letra)
            {
                case 'C':
                    return new[]
                    {
                        " " + new string(Bloque, 4),
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 1) + "    ",
                        " " + new string(Bloque, 4)
                    };
                case 'E':
                    return new[]
                    {
                        new string(Bloque, 5),
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 4) + " ",
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 5)
                    };
                case 'L':
                    return new[]
                    {
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 1) + "    ",
                        new string(Bloque, 5)
                    };
                case 'U':
                    return new[]
                    {
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        " " + new string(Bloque, 3) + " "
                    };
                case 'A':
                    return new[]
                    {
                        " " + new string(Bloque, 3) + " ",
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 5),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1)
                    };
                case 'R':
                    return new[]
                    {
                        new string(Bloque, 4) + " ",
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 4) + " ",
                        new string(Bloque, 1) + "  " + new string(Bloque, 1) + " ",
                        new string(Bloque, 1) + "   " + new string(Bloque, 1)
                    };
                case 'S':
                    return new[]
                    {
                        " " + new string(Bloque, 4),
                        new string(Bloque, 1) + "    ",
                        " " + new string(Bloque, 3) + " ",
                        "    " + new string(Bloque, 1),
                        new string(Bloque, 4) + " "
                    };
                case 'M':
                    return new[]
                    {
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 2) + " " + new string(Bloque, 2),
                        new string(Bloque, 1) + " " + new string(Bloque, 1) + " " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1)
                    };
                case 'O':
                    return new[]
                    {
                        " " + new string(Bloque, 3) + " ",
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        " " + new string(Bloque, 3) + " "
                    };
                case 'N':
                    return new[]
                    {
                        new string(Bloque, 1) + "   " + new string(Bloque, 1),
                        new string(Bloque, 2) + "  " + new string(Bloque, 1),
                        new string(Bloque, 1) + " " + new string(Bloque, 1) + " " + new string(Bloque, 1),
                        new string(Bloque, 1) + "  " + new string(Bloque, 2),
                        new string(Bloque, 1) + "   " + new string(Bloque, 1)
                    };
                case 'T':
                    return new[]
                    {
                        new string(Bloque, 5),
                        "  " + new string(Bloque, 1) + "  ",
                        "  " + new string(Bloque, 1) + "  ",
                        "  " + new string(Bloque, 1) + "  ",
                        "  " + new string(Bloque, 1) + "  "
                    };
                default:
                    return new[] { "     ", "     ", "     ", "     ", "     " };
            }
        }
    }
}
