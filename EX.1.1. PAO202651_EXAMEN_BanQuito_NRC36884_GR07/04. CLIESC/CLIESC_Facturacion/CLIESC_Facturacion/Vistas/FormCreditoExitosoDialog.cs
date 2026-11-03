using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormCreditoExitosoDialog : Form
    {
        private static readonly Color Rojo = Color.FromArgb(239, 68, 68);

        public FormCreditoExitosoDialog(
            RespuestaAmortizacion amortizacion,
            int codCliente,
            int numCuotas,
            decimal total)
        {
            Text = "Credito Emitido";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(560, 700);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(24, 12, 24, 14)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 132));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            Controls.Add(raiz);

            var titulo = new Label
            {
                Text = "Credito Emitido",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            var logo = Estilos.CrearIcono(150, 36, "logo_banquito.jpg");
            logo.Anchor = AnchorStyles.None;
            raiz.Controls.Add(logo, 0, 1);

            raiz.Controls.Add(CrearResumen(amortizacion.NumCredito, codCliente, numCuotas, total), 0, 2);

            var lblTabla = new Label
            {
                Text = "Tabla de Amortizacion",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(lblTabla, 0, 3);

            var grid = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(grid);
            var colNumero = Estilos.ColumnaTexto("Numero", "# Cuota", 70, true);
            var colValor = Estilos.ColumnaTexto("Valor", "Valor Cuota", 100, true);
            var colInteres = Estilos.ColumnaTexto("Interes", "Interes Pagado", 110, true);
            var colCapital = Estilos.ColumnaTexto("Capital", "Capital Pagado", 110, true);
            var colSaldo = Estilos.ColumnaTexto("Saldo", "Saldo", 100, true);
            colValor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colInteres.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCapital.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSaldo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colNumero.FillWeight = 12;
            colValor.FillWeight = 22;
            colInteres.FillWeight = 22;
            colCapital.FillWeight = 22;
            colSaldo.FillWeight = 22;
            grid.Columns.Add(colNumero);
            grid.Columns.Add(colValor);
            grid.Columns.Add(colInteres);
            grid.Columns.Add(colCapital);
            grid.Columns.Add(colSaldo);

            foreach (var cuota in amortizacion.Cuotas)
            {
                grid.Rows.Add(
                    cuota.NumCuota,
                    cuota.ValorCuota.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.InteresPagado.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.CapitalPagado.ToString("N2", CultureInfo.CurrentCulture),
                    cuota.Saldo.ToString("N2", CultureInfo.CurrentCulture));
            }

            raiz.Controls.Add(grid, 0, 4);

            var panelCerrar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(0, 8, 0, 0)
            };
            var btnCerrar = Estilos.CrearBoton("Cerrar", Rojo, 150, 38);
            btnCerrar.Anchor = AnchorStyles.None;
            btnCerrar.DialogResult = DialogResult.OK;
            panelCerrar.Controls.Add(btnCerrar, 0, 0);
            raiz.Controls.Add(panelCerrar, 0, 5);

            AcceptButton = btnCerrar;
            CancelButton = btnCerrar;
        }

        private static TableLayoutPanel CrearResumen(int numCredito, int codCliente, int numCuotas, decimal total)
        {
            var resumen = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Estilos.Fondo
            };
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

            resumen.Controls.Add(CrearEtiqueta("N° de credito"), 0, 0);
            resumen.Controls.Add(CrearEtiqueta("Cliente"), 1, 0);
            resumen.Controls.Add(CrearValor(numCredito.ToString(CultureInfo.CurrentCulture)), 0, 1);
            resumen.Controls.Add(CrearValor(codCliente.ToString(CultureInfo.CurrentCulture)), 1, 1);

            var panelCuotas = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };
            var lblCuotas = new Label
            {
                Text = "Cuotas",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panelCuotas.Controls.Add(lblCuotas);
            var lblCuotasValor = new Label
            {
                Text = numCuotas.ToString(CultureInfo.CurrentCulture),
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(0, 18)
            };
            panelCuotas.Controls.Add(lblCuotasValor);
            resumen.Controls.Add(panelCuotas, 0, 2);

            var panelTotal = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };
            var lblTotal = new Label
            {
                Text = "Total:",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panelTotal.Controls.Add(lblTotal);
            var lblTotalValor = new Label
            {
                Text = "$" + total.ToString("N2", CultureInfo.CurrentCulture),
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(0, 18)
            };
            panelTotal.Controls.Add(lblTotalValor);
            resumen.Controls.Add(panelTotal, 1, 2);

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
