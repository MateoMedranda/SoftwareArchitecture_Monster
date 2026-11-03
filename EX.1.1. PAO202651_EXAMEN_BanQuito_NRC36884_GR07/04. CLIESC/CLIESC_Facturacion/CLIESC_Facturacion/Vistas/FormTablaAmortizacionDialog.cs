using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormTablaAmortizacionDialog : Form
    {
        public FormTablaAmortizacionDialog(Factura factura, RespuestaAmortizacion amortizacion)
        {
            Text = "Tabla de Amortizacion";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            Size = new Size(640, 680);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(24, 12, 24, 14)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(raiz);

            var titulo = new Label
            {
                Text = "Tabla de Amortizacion",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            raiz.Controls.Add(CrearResumen(factura, amortizacion), 0, 1);

            var lblCreditos = new Label
            {
                Text = "Cuotas del credito",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(lblCreditos, 0, 2);

            var grid = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(grid);
            var colNumero = Estilos.ColumnaTexto("Numero", "# Cuota", 70, true);
            var colValor = Estilos.ColumnaTexto("Valor", "Valor Cuota", 100, true);
            var colInteres = Estilos.ColumnaTexto("Interes", "Interes Pagado", 110, true);
            var colCapital = Estilos.ColumnaTexto("Capital", "Capital Pagado", 110, true);
            var colSaldo = Estilos.ColumnaTexto("Saldo", "Saldo", 100, true);
            var colEstado = Estilos.ColumnaTexto("Estado", "Estado", 95, true);
            colValor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colInteres.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCapital.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSaldo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colEstado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNumero.FillWeight = 10;
            colValor.FillWeight = 18;
            colInteres.FillWeight = 20;
            colCapital.FillWeight = 20;
            colSaldo.FillWeight = 16;
            colEstado.FillWeight = 16;
            grid.Columns.Add(colNumero);
            grid.Columns.Add(colValor);
            grid.Columns.Add(colInteres);
            grid.Columns.Add(colCapital);
            grid.Columns.Add(colSaldo);
            grid.Columns.Add(colEstado);

            var cuotas = amortizacion.Cuotas ?? new System.Collections.Generic.List<CuotaAmortizacion>();
            foreach (var cuota in cuotas)
            {
                grid.Rows.Add(
                    cuota.NumCuota,
                    cuota.ValorCuota.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.InteresPagado.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.CapitalPagado.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.Saldo.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.Pagada ? "Pagado" : "Pendiente");
            }

            raiz.Controls.Add(grid, 0, 3);
        }

        private static TableLayoutPanel CrearResumen(Factura factura, RespuestaAmortizacion amortizacion)
        {
            var resumen = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Estilos.Fondo
            };
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));

            resumen.Controls.Add(CrearEtiqueta("N° de credito"), 0, 0);
            resumen.Controls.Add(CrearEtiqueta("Factura"), 1, 0);
            resumen.Controls.Add(CrearValor(amortizacion.NumCredito.ToString(CultureInfo.CurrentCulture)), 0, 1);
            resumen.Controls.Add(CrearValor("N° " + factura.NumFactura.ToString(CultureInfo.CurrentCulture)), 1, 1);

            return resumen;
        }

        private static Label CrearEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                ForeColor = Estilos.TextoSecundario,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 8.5F)
            };
        }

        private static Label CrearValor(string texto)
        {
            return new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                ForeColor = Estilos.Texto,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
        }
    }
}
