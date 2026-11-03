using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using CLIESC_Facturacion.Servicios;

namespace CLIESC_Facturacion.Vistas
{
    internal static class Estilos
    {
        public static readonly Color Fondo = Color.FromArgb(250, 250, 250);
        public static readonly Color Texto = Color.FromArgb(31, 41, 55);
        public static readonly Color TextoSecundario = Color.FromArgb(107, 114, 128);
        public static readonly Color Morado = Color.FromArgb(139, 92, 246);
        public static readonly Color Azul = Color.FromArgb(59, 130, 246);
        public static readonly Color Verde = Color.FromArgb(34, 197, 94);
        public static readonly Color Amarillo = Color.FromArgb(202, 138, 4);
        public static readonly Color Borde = Color.FromArgb(229, 231, 235);

        public static Button CrearBoton(string texto, Color color, int ancho, int alto)
        {
            var boton = new Button
            {
                Text = texto,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Size = new Size(ancho, alto),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = Oscurecer(color, 0.08f);
            return boton;
        }

        public static Button CrearBotonSecundario(string texto, int ancho, int alto)
        {
            var boton = CrearBoton(texto, Color.White, ancho, alto);
            boton.ForeColor = Texto;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = Borde;
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            return boton;
        }

        public static Button CrearBotonNavegacion(string texto, bool activo)
        {
            var boton = new Button
            {
                Text = texto,
                AutoSize = false,
                Size = new Size(100, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Fondo,
                ForeColor = activo ? Texto : TextoSecundario,
                Font = new Font("Segoe UI", 9.5F, activo ? FontStyle.Bold : FontStyle.Regular),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            return boton;
        }

        public static Label CrearTitulo(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Texto,
                AutoSize = true
            };
        }

        public static PictureBox CrearLogo(int ancho, int alto)
        {
            var logo = new PictureBox
            {
                Image = CargarLogo(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(ancho, alto),
                BackColor = Color.Transparent
            };
            return logo;
        }

        public static PictureBox CrearIcono(int ancho, int alto, String nombreArchivo)
        {
            var logo = new PictureBox
            {
                Image = CargarIcono(nombreArchivo),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(ancho, alto),
                BackColor = Color.Transparent
            };
            return logo;
        }

        public static void ConfigurarGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextoSecundario;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(249, 250, 251);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoSecundario;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            grid.ColumnHeadersHeight = 38;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.DefaultCellStyle.ForeColor = Texto;
            grid.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            grid.DefaultCellStyle.SelectionForeColor = Texto;
            grid.RowTemplate.Height = 38;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.ReadOnly = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static DataGridViewTextBoxColumn ColumnaTexto(string nombre, string encabezado, int ancho, bool soloLectura)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = nombre,
                HeaderText = encabezado,
                Width = ancho,
                ReadOnly = soloLectura,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        public static DataGridViewButtonColumn ColumnaBoton(string nombre, string texto, int ancho)
        {
            return new DataGridViewButtonColumn
            {
                Name = nombre,
                HeaderText = string.Empty,
                Text = texto,
                UseColumnTextForButtonValue = true,
                Width = ancho,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Texto,
                    Font = new Font("Segoe UI", 8.5F)
                },
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        public static void MostrarError(IWin32Window propietario, Exception excepcion)
        {
            var mensaje = excepcion is ApiException
                ? excepcion.Message
                : "Ocurrio un error inesperado: " + excepcion.Message;

            MessageBox.Show(
                propietario,
                mensaje,
                "Celulares Monster",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static Image CargarLogo()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidatos = new[]
            {
                Path.Combine(baseDir, "resources", "logo.png"),
                Path.Combine(baseDir, "..", "..", "resources", "logo.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "resources", "logo.png")
            };

            foreach (var candidato in candidatos)
            {
                try
                {
                    if (!File.Exists(candidato))
                    {
                        continue;
                    }

                    using (var stream = new FileStream(candidato, FileMode.Open, FileAccess.Read))
                    using (var imagen = Image.FromStream(stream))
                    {
                        return new Bitmap(imagen);
                    }
                }
                catch (Exception)
                {
                    // Prueba con la siguiente ubicacion.
                }
            }

            return CrearLogoRespaldo();
        }

        private static Image CargarIcono(String nombreArchivo)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidatos = new[]
            {
                Path.Combine(baseDir, "resources", nombreArchivo),
                Path.Combine(baseDir, "..", "..", "resources", nombreArchivo),
                Path.Combine(Directory.GetCurrentDirectory(), "resources", nombreArchivo)
            };

            foreach (var candidato in candidatos)
            {
                try
                {
                    if (!File.Exists(candidato))
                    {
                        continue;
                    }

                    using (var stream = new FileStream(candidato, FileMode.Open, FileAccess.Read))
                    using (var imagen = Image.FromStream(stream))
                    {
                        return new Bitmap(imagen);
                    }
                }
                catch (Exception)
                {
                    // Prueba con la siguiente ubicacion.
                }
            }

            return CrearLogoRespaldo();
        }

        private static Image CrearLogoRespaldo()
        {
            var imagen = new Bitmap(420, 150);
            using (var g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(31, 36, 48));
                using (var fuente = new Font("Segoe UI", 26F, FontStyle.Bold))
                using (var pincel = new SolidBrush(Color.FromArgb(96, 205, 255)))
                {
                    g.DrawString("Celulares", fuente, pincel, 22, 18);
                }
                using (var fuente = new Font("Segoe UI", 30F, FontStyle.Bold | FontStyle.Italic))
                using (var pincel = new SolidBrush(Color.FromArgb(196, 132, 252)))
                {
                    g.DrawString("Monster", fuente, pincel, 52, 64);
                }
            }
            return imagen;
        }

        private static Color Oscurecer(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * (1 - factor)),
                (int)(color.G * (1 - factor)),
                (int)(color.B * (1 - factor)));
        }
    }
}
