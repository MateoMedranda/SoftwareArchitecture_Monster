using System.Drawing;
using System.Windows.Forms;
using CLIESC_Facturacion.Config;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormLogin : Form
    {
        private readonly TextBox _usuario = new TextBox { Width = 300, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
        private readonly TextBox _contrasena = new TextBox { Width = 300, Font = new Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
        private readonly Label _error = new Label { AutoSize = false, ForeColor = Color.FromArgb(185, 28, 28), Font = new Font("Segoe UI", 9F), TextAlign = ContentAlignment.MiddleCenter, Width = 300, Height = 24 };

        public FormLogin()
        {
            Text = "Celulares Monster · Acceso";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 243, 255);
            ClientSize = new Size(430, 455);

            var tarjeta = new Panel { BackColor = Color.White, Location = new Point(28, 28), Size = new Size(374, 399) };
            var franja = new Panel { BackColor = Color.FromArgb(124, 58, 237), Dock = DockStyle.Top, Height = 112 };
            var icono = new Label { Text = "M", ForeColor = Color.FromArgb(124, 58, 237), BackColor = Color.White, Font = new Font("Segoe UI", 20F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Size = new Size(54, 54), Location = new Point(160, 18) };
            var titulo = new Label { Text = "Celulares Monster", ForeColor = Color.White, Font = new Font("Segoe UI", 17F, FontStyle.Bold), AutoSize = true, Location = new Point(91, 77) };
            var subtitulo = new Label { Text = "Inicie sesión para continuar", ForeColor = Color.FromArgb(107, 114, 128), Font = new Font("Segoe UI", 10F), AutoSize = true, Location = new Point(37, 137) };
            var etiquetaUsuario = new Label { Text = "Usuario", Font = new Font("Segoe UI", 9F, FontStyle.Bold), AutoSize = true, Location = new Point(37, 177) };
            _usuario.Location = new Point(37, 198);
            var etiquetaContrasena = new Label { Text = "Contraseña", Font = new Font("Segoe UI", 9F, FontStyle.Bold), AutoSize = true, Location = new Point(37, 246) };
            _contrasena.Location = new Point(37, 267);
            _error.Location = new Point(37, 303);
            var ingresar = new Button { Text = "Ingresar", Width = 300, Height = 42, Location = new Point(37, 337), BackColor = Color.FromArgb(139, 92, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            ingresar.FlatAppearance.BorderSize = 0;
            ingresar.Click += (_, __) => Ingresar();
            AcceptButton = ingresar;

            franja.Controls.AddRange(new Control[] { icono, titulo });
            tarjeta.Controls.AddRange(new Control[] { franja, subtitulo, etiquetaUsuario, _usuario, etiquetaContrasena, _contrasena, _error, ingresar });
            Controls.Add(tarjeta);
        }

        private void Ingresar()
        {
            if (AutenticacionMock.CredencialesValidas(_usuario.Text.Trim(), _contrasena.Text))
            {
                DialogResult = DialogResult.OK;
                return;
            }

            _error.Text = "Usuario o contraseña incorrectos.";
            _contrasena.SelectAll();
            _contrasena.Focus();
        }
    }
}
