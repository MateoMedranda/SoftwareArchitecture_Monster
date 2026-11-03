using System;
using System.Drawing;
using System.Windows.Forms;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormClienteDialog : Form
    {
        private readonly Cliente _cliente;
        private readonly TextBox _txtCedula;
        private readonly TextBox _txtNombre;

        public FormClienteDialog(Cliente cliente)
        {
            _cliente = cliente == null ? new Cliente() : cliente.Copiar();

            Text = cliente == null ? "Agregar cliente" : "Editar cliente";
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

            var lblCedula = new Label
            {
                Text = "Cedula",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(30, 72)
            };
            Controls.Add(lblCedula);

            _txtCedula = new TextBox
            {
                Location = new Point(30, 94),
                Size = new Size(390, 26),
                MaxLength = 10,
                Text = _cliente.Cedula
            };
            Controls.Add(_txtCedula);

            var lblNombre = new Label
            {
                Text = "Nombre completo",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(30, 132)
            };
            Controls.Add(lblNombre);

            _txtNombre = new TextBox
            {
                Location = new Point(30, 154),
                Size = new Size(390, 26),
                MaxLength = 100,
                Text = _cliente.Nombre
            };
            Controls.Add(_txtNombre);

            var btnGuardar = Estilos.CrearBoton("Guardar", Estilos.Morado, 110, 36);
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

        public Cliente Cliente
        {
            get { return _cliente; }
        }

        private void GuardarClick(object sender, EventArgs e)
        {
            var cedula = _txtCedula.Text.Trim();
            var nombre = _txtNombre.Text.Trim();

            if (cedula.Length != 10)
            {
                MessageBox.Show(this, "La cedula debe tener exactamente 10 caracteres.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCedula.Focus();
                return;
            }

            if (nombre.Length == 0)
            {
                MessageBox.Show(this, "El nombre es obligatorio.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtNombre.Focus();
                return;
            }

            _cliente.Cedula = cedula;
            _cliente.Nombre = nombre;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
