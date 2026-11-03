using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CLIESC_Facturacion.Config;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormInicio : Form
    {
        public FormInicio()
        {
            Text = "Celulares Monster - Inicio";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 620);
            MinimumSize = new Size(860, 540);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0, 22, 0, 12)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            Controls.Add(raiz);

            var logo = Estilos.CrearLogo(340, 150);
            var iconoFacturas = CrearIconoTarjeta("bill.png");
            var iconoClientes = CrearIconoTarjeta("person.png");
            var iconoCatalogo = CrearIconoTarjeta("phone.png");
            logo.Anchor = AnchorStyles.None;
            raiz.Controls.Add(logo, 0, 0);

            var subtitulo = new Label
            {
                Text = "Sistema de facturacion",
                Font = new Font("Segoe UI", 15F, FontStyle.Regular),
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Anchor = AnchorStyles.None
            };
            raiz.Controls.Add(subtitulo, 0, 1);

            var tarjetas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Estilos.Fondo,
                Padding = new Padding(70, 12, 70, 18)
            };
            tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tarjetas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.Controls.Add(tarjetas, 0, 2);

            tarjetas.Controls.Add(CrearTarjeta(iconoFacturas, "Facturas", Estilos.Verde, FacturasClick), 0, 0);
            tarjetas.Controls.Add(CrearTarjeta(iconoClientes, "Clientes", Estilos.Amarillo, ClientesClick), 1, 0);
            tarjetas.Controls.Add(CrearTarjeta(iconoCatalogo, "Catalogo", Estilos.Azul, CatalogoClick), 2, 0);

            var pie = new Label
            {
                Text = "API: " + AppConfig.ApiBaseUrl +
                       (string.IsNullOrWhiteSpace(AppConfig.EnvFilePath)
                           ? "  |  Configuracion: valores por defecto"
                           : "  |  Configuracion: " + AppConfig.EnvFilePath),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Estilos.TextoSecundario,
                Font = new Font("Segoe UI", 8F)
            };
            raiz.Controls.Add(pie, 0, 3);
        }

        private Button CrearTarjeta(Image icono, string titulo, Color color, EventHandler click)
        {
            var boton = new Button
            {
                Text = titulo,
                Image = icono,
                Dock = DockStyle.Fill,
                Margin = new Padding(14),
                Padding = new Padding(0, 18, 0, 20),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.BottomCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                UseVisualStyleBackColor = false
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Max(0, color.R - 14),
                Math.Max(0, color.G - 14),
                Math.Max(0, color.B - 14));
            boton.Click += click;
            return boton;
        }

        private static Image CrearIconoTarjeta(string nombreArchivo)
        {
            const int tamano = 64;
            var resultado = new Bitmap(tamano, tamano);

            using (var pictureBox = Estilos.CrearIcono(tamano, tamano, nombreArchivo))
            using (var grafico = Graphics.FromImage(resultado))
            {
                grafico.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grafico.SmoothingMode = SmoothingMode.AntiAlias;
                grafico.PixelOffsetMode = PixelOffsetMode.HighQuality;

                if (pictureBox.Image != null)
                {
                    grafico.DrawImage(pictureBox.Image, new Rectangle(0, 0, tamano, tamano));
                    pictureBox.Image.Dispose();
                }
            }

            return resultado;
        }

        private void FacturasClick(object sender, EventArgs e)
        {
            Navegacion.Abrir(this, new FormFacturas());
        }

        private void ClientesClick(object sender, EventArgs e)
        {
            Navegacion.Abrir(this, new FormClientes());
        }

        private void CatalogoClick(object sender, EventArgs e)
        {
            Navegacion.Abrir(this, new FormCatalogo());
        }
    }
}
