using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormConfirmarEfectivoDialog : Form
    {
        private static readonly Color Rojo = Color.FromArgb(239, 68, 68);

        public FormConfirmarEfectivoDialog(decimal totalConDescuento)
        {
            Text = "Pago en efectivo";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(460, 220);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(24, 14, 24, 14)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            Controls.Add(raiz);

            var titulo = new Label
            {
                Text = "Pago en efectivo",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            var mensaje = new Label
            {
                Text = "El total a pagar es de $" +
                       totalConDescuento.ToString("N2", CultureInfo.CurrentCulture) +
                       " con el descuento del 42%",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Estilos.Texto,
                Font = new Font("Segoe UI", 10F)
            };
            raiz.Controls.Add(mensaje, 0, 1);

            var botones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var btnConfirmar = Estilos.CrearBoton("Confirmar", Estilos.Azul, 150, 38);
            btnConfirmar.Anchor = AnchorStyles.None;
            btnConfirmar.DialogResult = DialogResult.OK;
            botones.Controls.Add(btnConfirmar, 0, 0);

            var btnCancelar = Estilos.CrearBoton("Cancelar", Rojo, 150, 38);
            btnCancelar.Anchor = AnchorStyles.None;
            btnCancelar.DialogResult = DialogResult.Cancel;
            botones.Controls.Add(btnCancelar, 1, 0);

            raiz.Controls.Add(botones, 0, 2);

            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;
        }
    }
}
