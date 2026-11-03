using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLIESC_Facturacion.Controladores;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormCatalogo : Form
    {
        private readonly ProductoControlador _controlador;
        private readonly DataGridView _grid;
        private readonly TextBox _txtBusqueda;
        private readonly Label _lblEstado;

        public FormCatalogo()
        {
            _controlador = new ProductoControlador();

            Text = "Celulares Monster - Catalogo";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 680);
            MinimumSize = new Size(860, 560);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 5
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            Controls.Add(raiz);

            raiz.Controls.Add(CrearEncabezado(), 0, 0);
            raiz.Controls.Add(CrearNavegacion(), 0, 1);

            var barra = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(45, 16, 45, 8)
            };
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblBuscar = new Label
            {
                Text = "Buscar por codigo",
                AutoSize = true,
                ForeColor = Estilos.Texto,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 14, 0)
            };
            barra.Controls.Add(lblBuscar, 0, 0);

            _txtBusqueda = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 8, 0),
                Font = new Font("Segoe UI", 10F)
            };
            _txtBusqueda.KeyDown += async (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    args.SuppressKeyPress = true;
                    await BuscarAsync();
                }
            };
            barra.Controls.Add(_txtBusqueda, 1, 0);

            var btnBuscar = Estilos.CrearBoton("Buscar", Estilos.Texto, 86, 34);
            btnBuscar.Margin = new Padding(0, 3, 12, 0);
            btnBuscar.Click += async (sender, args) => await BuscarAsync();
            barra.Controls.Add(btnBuscar, 2, 0);

            var btnAgregar = Estilos.CrearBoton("+  Agregar celular", Estilos.Azul, 172, 36);
            btnAgregar.Margin = new Padding(0, 2, 0, 0);
            btnAgregar.Click += async (sender, args) => await AgregarAsync();
            barra.Controls.Add(btnAgregar, 3, 0);
            raiz.Controls.Add(barra, 0, 2);

            var contenedorGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(45, 8, 45, 12),
                BackColor = Estilos.Fondo
            };
            raiz.Controls.Add(contenedorGrid, 0, 3);

            _grid = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(_grid);
            var colNum = Estilos.ColumnaTexto("Num", "Num", 55, true);
            var colNombre = Estilos.ColumnaTexto("Nombre", "Nombre", 330, true);
            var colPrecio = Estilos.ColumnaTexto("Precio", "Precio", 140, true);
            colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colNum.FillWeight = 8;
            colNombre.FillWeight = 55;
            colPrecio.FillWeight = 20;
            _grid.Columns.Add(colNum);
            _grid.Columns.Add(colNombre);
            _grid.Columns.Add(colPrecio);
            _grid.Columns.Add(Estilos.ColumnaBoton("Editar", "Editar", 90));
            _grid.Columns.Add(Estilos.ColumnaBoton("Eliminar", "Eliminar", 90));
            _grid.Columns["Editar"].FillWeight = 12;
            _grid.Columns["Eliminar"].FillWeight = 12;
            _grid.CellContentClick += GridCellContentClick;
            contenedorGrid.Controls.Add(_grid);

            _lblEstado = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(48, 0, 0, 0),
                ForeColor = Estilos.TextoSecundario,
                Font = new Font("Segoe UI", 8F),
                Text = "Cargando catalogo..."
            };
            raiz.Controls.Add(_lblEstado, 0, 4);

            Shown += async (sender, args) => await CargarAsync();
        }

        private Panel CrearEncabezado()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var logo = Estilos.CrearLogo(210, 96);
            logo.Location = new Point(42, 20);
            panel.Controls.Add(logo);

            var titulo = Estilos.CrearTitulo("Catalogo");
            titulo.Location = new Point(300, 42);
            panel.Controls.Add(titulo);

            var subtitulo = new Label
            {
                Text = "Gestiona los celulares disponibles para facturar",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(306, 94)
            };
            panel.Controls.Add(subtitulo);

            return panel;
        }

        private Panel CrearNavegacion()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var btnFacturas = Estilos.CrearBotonNavegacion("Facturas", false);
            btnFacturas.Location = new Point(42, 7);
            btnFacturas.Click += (sender, args) => Navegacion.Abrir(this, new FormFacturas());
            panel.Controls.Add(btnFacturas);

            var btnClientes = Estilos.CrearBotonNavegacion("Clientes", false);
            btnClientes.Location = new Point(142, 7);
            btnClientes.Click += (sender, args) => Navegacion.Abrir(this, new FormClientes());
            panel.Controls.Add(btnClientes);

            var btnCatalogo = Estilos.CrearBotonNavegacion("Catalogo", true);
            btnCatalogo.Location = new Point(242, 7);
            panel.Controls.Add(btnCatalogo);

            var linea = new Panel
            {
                BackColor = Estilos.Borde,
                Height = 1,
                Dock = DockStyle.Bottom
            };
            panel.Controls.Add(linea);

            return panel;
        }

        private async Task CargarAsync()
        {
            try
            {
                UseWaitCursor = true;
                var productos = await _controlador.ListarAsync();
                MostrarProductos(productos);
                _lblEstado.Text = productos.Count + " producto(s) en el catalogo.";
            }
            catch (Exception ex)
            {
                _lblEstado.Text = "No se pudo cargar el catalogo.";
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void MostrarProductos(IEnumerable<Producto> productos)
        {
            _grid.Rows.Clear();
            var numero = 1;
            foreach (var producto in productos)
            {
                var indice = _grid.Rows.Add(
                    numero,
                    producto.Nombre,
                    producto.Precio.ToString("N2", CultureInfo.CurrentCulture),
                    "Editar",
                    "Eliminar");
                _grid.Rows[indice].Tag = producto;
                numero++;
            }
        }

        private async Task BuscarAsync()
        {
            var texto = _txtBusqueda.Text.Trim();
            if (texto.Length == 0)
            {
                await CargarAsync();
                return;
            }

            int codigo;
            if (!int.TryParse(texto, out codigo))
            {
                MessageBox.Show(this, "El codigo debe ser un numero entero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                var producto = await _controlador.BuscarAsync(codigo);
                MostrarProductos(new[] { producto });
                _lblEstado.Text = "1 resultado encontrado.";
            }
            catch (Exception ex)
            {
                _lblEstado.Text = "Sin resultados.";
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task AgregarAsync()
        {
            using (var dialogo = new FormProductoDialog(null))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    var respuesta = await _controlador.CrearAsync(dialogo.Producto);
                    MessageBox.Show(this, respuesta.Mensaje, "Producto creado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarAsync();
                }
                catch (Exception ex)
                {
                    Estilos.MostrarError(this, ex);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async Task EditarAsync(Producto producto)
        {
            using (var dialogo = new FormProductoDialog(producto))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    var respuesta = await _controlador.ActualizarAsync(dialogo.Producto);
                    MessageBox.Show(this, respuesta.Mensaje, "Producto actualizado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarAsync();
                }
                catch (Exception ex)
                {
                    Estilos.MostrarError(this, ex);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async Task EliminarAsync(Producto producto)
        {
            var confirmacion = MessageBox.Show(
                this,
                "Eliminar el producto " + producto.Nombre + "?",
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                await _controlador.EliminarAsync(producto.NumInventario);
                await CargarAsync();
            }
            catch (Exception ex)
            {
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void GridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var producto = _grid.Rows[e.RowIndex].Tag as Producto;
            if (producto == null)
            {
                return;
            }

            var columna = _grid.Columns[e.ColumnIndex].Name;
            if (columna == "Editar")
            {
                await EditarAsync(producto);
            }
            else if (columna == "Eliminar")
            {
                await EliminarAsync(producto);
            }
        }
    }
}
