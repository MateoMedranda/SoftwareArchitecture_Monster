using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormProductoDialog : Form
    {
        private readonly Producto _producto;
        private readonly TextBox _txtNombre;
        private readonly TextBox _txtPrecio;

        public FormProductoDialog(Producto producto)
        {
            _producto = producto == null ? new Producto() : producto.Copiar();

            Text = producto == null ? "Agregar celular" : "Editar celular";
            Size = new Size(460, 280);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var titulo = new Label
            {
                Text = Text,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Location = new Point(28, 22)
            };
            Controls.Add(titulo);

            var lblNombre = new Label
            {
                Text = "Nombre del equipo",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(30, 72)
            };
            Controls.Add(lblNombre);

            _txtNombre = new TextBox
            {
                Location = new Point(30, 94),
                Size = new Size(390, 26),
                MaxLength = 100,
                Text = _producto.Nombre
            };
            Controls.Add(_txtNombre);

            var lblPrecio = new Label
            {
                Text = "Precio",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(30, 132)
            };
            Controls.Add(lblPrecio);

            _txtPrecio = new TextBox
            {
                Location = new Point(30, 154),
                Size = new Size(180, 26),
                Text = producto == null
                    ? string.Empty
                    : _producto.Precio.ToString("0.00", CultureInfo.InvariantCulture)
            };
            Controls.Add(_txtPrecio);

            var btnGuardar = Estilos.CrearBoton("Guardar", Estilos.Azul, 110, 36);
            btnGuardar.Location = new Point(200, 205);
            btnGuardar.Click += GuardarClick;
            Controls.Add(btnGuardar);

            var btnCancelar = Estilos.CrearBotonSecundario("Cancelar", 110, 36);
            btnCancelar.Location = new Point(320, 205);
            btnCancelar.DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
        }

        public Producto Producto
        {
            get { return _producto; }
        }

        private void GuardarClick(object sender, EventArgs e)
        {
            var nombre = _txtNombre.Text.Trim();
            if (nombre.Length == 0)
            {
                MessageBox.Show(this, "El nombre del equipo es obligatorio.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtNombre.Focus();
                return;
            }

            decimal precio;
            var texto = _txtPrecio.Text.Trim();
            if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out precio) &&
                !decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out precio))
            {
                MessageBox.Show(this, "El precio debe ser un valor numerico.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPrecio.Focus();
                return;
            }

            if (precio <= 0)
            {
                MessageBox.Show(this, "El precio debe ser mayor que cero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPrecio.Focus();
                return;
            }

            _producto.Nombre = nombre;
            _producto.Precio = precio;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
