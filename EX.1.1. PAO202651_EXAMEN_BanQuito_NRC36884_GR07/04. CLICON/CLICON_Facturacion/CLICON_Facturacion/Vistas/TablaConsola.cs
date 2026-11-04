using System;
using System.Collections.Generic;
using System.Text;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Tabla ASCII sencilla para mostrar listados en la consola.
    /// </summary>
    public sealed class TablaConsola
    {
        private const int AnchoMaximoColumna = 34;

        private readonly string[] _encabezados;
        private readonly List<string[]> _filas;

        public TablaConsola(params string[] encabezados)
        {
            if (encabezados == null || encabezados.Length == 0)
            {
                throw new ArgumentException("La tabla necesita al menos una columna.", "encabezados");
            }

            _encabezados = new string[encabezados.Length];
            for (var i = 0; i < encabezados.Length; i++)
            {
                _encabezados[i] = encabezados[i] ?? string.Empty;
            }

            _filas = new List<string[]>();
        }

        public void Agregar(params object[] celdas)
        {
            var fila = new string[_encabezados.Length];
            for (var i = 0; i < fila.Length; i++)
            {
                fila[i] = celdas != null && i < celdas.Length && celdas[i] != null
                    ? celdas[i].ToString()
                    : string.Empty;
            }

            _filas.Add(fila);
        }

        public void Imprimir()
        {
            var anchos = CalcularAnchos();

            ImprimirSeparador(anchos);
            ImprimirFila(_encabezados, anchos);
            ImprimirSeparador(anchos);

            foreach (var fila in _filas)
            {
                ImprimirFila(fila, anchos);
            }

            ImprimirSeparador(anchos);
        }

        private int[] CalcularAnchos()
        {
            var anchos = new int[_encabezados.Length];
            for (var i = 0; i < anchos.Length; i++)
            {
                var ancho = _encabezados[i].Length;
                foreach (var fila in _filas)
                {
                    ancho = Math.Max(ancho, fila[i].Length);
                }

                anchos[i] = Math.Min(AnchoMaximoColumna, Math.Max(4, ancho));
            }

            return anchos;
        }

        private static void ImprimirSeparador(int[] anchos)
        {
            var linea = new StringBuilder("+");
            foreach (var ancho in anchos)
            {
                linea.Append(new string('-', ancho + 2));
                linea.Append('+');
            }

            Console.WriteLine(linea.ToString());
        }

        private static void ImprimirFila(string[] celdas, int[] anchos)
        {
            var linea = new StringBuilder("|");
            for (var i = 0; i < anchos.Length; i++)
            {
                linea.Append(' ');
                linea.Append(Ajustar(celdas[i], anchos[i]));
                linea.Append(" |");
            }

            Console.WriteLine(linea.ToString());
        }

        private static string Ajustar(string valor, int ancho)
        {
            valor = (valor ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();

            if (valor.Length > ancho)
            {
                valor = ancho > 3
                    ? valor.Substring(0, ancho - 3) + "..."
                    : valor.Substring(0, ancho);
            }

            return valor.PadRight(ancho);
        }
    }
}
