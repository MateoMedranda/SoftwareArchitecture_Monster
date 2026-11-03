using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormConfirmarCreditoDialog : Form
    {
        private static readonly Color Rojo = Color.FromArgb(239, 68, 68);

        private readonly NumericUpDown _numCuotas;

        public FormConfirmarCreditoDialog(decimal valorFactura)
        {
            Text = "Credito Directo";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(440, 360);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(28, 12, 28, 14)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(raiz);

            var titulo = new Label
            {
                Text = "Credito Directo",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            var logo = Estilos.CrearIcono(140, 34, "logo_banquito.jpg");
            logo.Anchor = AnchorStyles.None;
            raiz.Controls.Add(logo, 0, 1);

            var panelValor = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };
            var lblValor = new Label
            {
                Text = "Valor factura:",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panelValor.Controls.Add(lblValor);

            var txtValor = new TextBox
            {
                Location = new Point(0, 22),
                Size = new Size(360, 26),
                ReadOnly = true,
                Text = "$" + valorFactura.ToString("N2", CultureInfo.CurrentCulture),
                BackColor = Color.White
            };
            panelValor.Controls.Add(txtValor);
            raiz.Controls.Add(panelValor, 0, 2);

            var panelCuotas = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };
            var lblCuotas = new Label
            {
                Text = "Cuotas",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panelCuotas.Controls.Add(lblCuotas);

            _numCuotas = new NumericUpDown
            {
                Location = new Point(0, 22),
                Size = new Size(110, 26),
                Minimum = 1,
                Maximum = 18,
                Value = 12,
                Increment = 1,
                TextAlign = HorizontalAlignment.Right
            };
            panelCuotas.Controls.Add(_numCuotas);
            raiz.Controls.Add(panelCuotas, 0, 3);

            var botones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 6, 0, 0)
            };
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var btnConfirmar = Estilos.CrearBoton("Confirmar", Estilos.Azul, 150, 38);
            btnConfirmar.Anchor = AnchorStyles.None;
            btnConfirmar.Click += ConfirmarClick;
            botones.Controls.Add(btnConfirmar, 0, 0);

            var btnCancelar = Estilos.CrearBoton("Cancelar", Rojo, 150, 38);
            btnCancelar.Anchor = AnchorStyles.None;
            btnCancelar.DialogResult = DialogResult.Cancel;
            botones.Controls.Add(btnCancelar, 1, 0);

            raiz.Controls.Add(botones, 0, 4);

            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;
        }

        public int NumCuotas
        {
            get { return (int)_numCuotas.Value; }
        }

        private void ConfirmarClick(object sender, EventArgs e)
        {
            if (_numCuotas.Value <= 0)
            {
                MessageBox.Show(this, "El numero de cuotas debe ser mayor que cero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _numCuotas.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
