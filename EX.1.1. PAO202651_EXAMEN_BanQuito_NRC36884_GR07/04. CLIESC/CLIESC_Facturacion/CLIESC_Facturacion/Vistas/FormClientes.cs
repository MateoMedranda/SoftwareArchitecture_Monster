using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLIESC_Facturacion.Controladores;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormClientes : Form
    {
        private readonly ClienteControlador _controlador;
        private readonly DataGridView _grid;
        private TextBox _txtBusqueda;
        private RadioButton _rdCedula;
        private RadioButton _rdCodigo;
        private readonly Label _lblEstado;

        public FormClientes()
        {
            _controlador = new ClienteControlador();

            Text = "Celulares Monster - Clientes";
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
            raiz.Controls.Add(CrearBarraBusqueda(), 0, 2);

            var contenedorGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(45, 8, 45, 12),
                BackColor = Estilos.Fondo
            };
            raiz.Controls.Add(contenedorGrid, 0, 3);

            _grid = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(_grid);
            _grid.Columns.Add(Estilos.ColumnaTexto("Num", "Num", 55, true));
            _grid.Columns.Add(Estilos.ColumnaTexto("Nombres", "Nombres", 300, true));
            _grid.Columns.Add(Estilos.ColumnaTexto("Cedula", "Cedula", 160, true));
            _grid.Columns.Add(Estilos.ColumnaBoton("Editar", "Editar", 90));
            _grid.Columns.Add(Estilos.ColumnaBoton("Eliminar", "Eliminar", 90));
            _grid.Columns["Editar"].FillWeight = 12;
            _grid.Columns["Eliminar"].FillWeight = 12;
            _grid.Columns["Nombres"].FillWeight = 52;
            _grid.Columns["Cedula"].FillWeight = 16;
            _grid.Columns["Num"].FillWeight = 8;
            _grid.CellContentClick += GridCellContentClick;
            contenedorGrid.Controls.Add(_grid);

            _lblEstado = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(48, 0, 0, 0),
                ForeColor = Estilos.TextoSecundario,
                Font = new Font("Segoe UI", 8F),
                Text = "Cargando clientes..."
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

            var titulo = Estilos.CrearTitulo("Clientes");
            titulo.Location = new Point(300, 42);
            panel.Controls.Add(titulo);

            var subtitulo = new Label
            {
                Text = "Consulta, registra, edita y elimina clientes",
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
            btnFacturas.Click += (sender, args) => MessageBox.Show(this,
                "La pantalla de facturas se implementara en la siguiente iteracion.",
                "Celulares Monster", MessageBoxButtons.OK, MessageBoxIcon.Information);
            panel.Controls.Add(btnFacturas);

            var btnClientes = Estilos.CrearBotonNavegacion("Clientes", true);
            btnClientes.Location = new Point(142, 7);
            panel.Controls.Add(btnClientes);

            var btnCatalogo = Estilos.CrearBotonNavegacion("Catalogo", false);
            btnCatalogo.Location = new Point(242, 7);
            btnCatalogo.Click += (sender, args) => Navegacion.Abrir(this, new FormCatalogo());
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

        private TableLayoutPanel CrearBarraBusqueda()
        {
            var barra = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 6,
                RowCount = 1,
                Padding = new Padding(45, 16, 45, 8)
            };
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblBuscar = new Label
            {
                Text = "Buscar por",
                AutoSize = true,
                ForeColor = Estilos.Texto,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 14, 0)
            };
            barra.Controls.Add(lblBuscar, 0, 0);

            _rdCedula = new RadioButton
            {
                Text = "Cedula",
                AutoSize = true,
                Checked = true,
                ForeColor = Estilos.Texto,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 12, 0)
            };
            barra.Controls.Add(_rdCedula, 1, 0);

            _rdCodigo = new RadioButton
            {
                Text = "Codigo",
                AutoSize = true,
                ForeColor = Estilos.Texto,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 16, 0)
            };
            barra.Controls.Add(_rdCodigo, 2, 0);

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
            barra.Controls.Add(_txtBusqueda, 3, 0);

            var btnBuscar = Estilos.CrearBoton("Buscar", Estilos.Texto, 86, 34);
            btnBuscar.Margin = new Padding(0, 3, 12, 0);
            btnBuscar.Click += async (sender, args) => await BuscarAsync();
            barra.Controls.Add(btnBuscar, 4, 0);

            var btnAgregar = Estilos.CrearBoton("+  Agregar cliente", Estilos.Morado, 172, 36);
            btnAgregar.Margin = new Padding(0, 2, 0, 0);
            btnAgregar.Click += async (sender, args) => await AgregarAsync();
            barra.Controls.Add(btnAgregar, 5, 0);

            return barra;
        }

        private async Task CargarAsync()
        {
            try
            {
                UseWaitCursor = true;
                var clientes = await _controlador.ListarAsync();
                MostrarClientes(clientes);
                _lblEstado.Text = clientes.Count + " cliente(s) registrado(s).";
            }
            catch (Exception ex)
            {
                _lblEstado.Text = "No se pudo cargar la lista.";
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void MostrarClientes(IEnumerable<Cliente> clientes)
        {
            _grid.Rows.Clear();
            foreach (var cliente in clientes)
            {
                var indice = _grid.Rows.Add(cliente.CodCliente, cliente.Nombre, cliente.Cedula, "Editar", "Eliminar");
                _grid.Rows[indice].Tag = cliente;
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

            try
            {
                UseWaitCursor = true;
                Cliente cliente;

                if (_rdCodigo.Checked)
                {
                    int codigo;
                    if (!int.TryParse(texto, out codigo))
                    {
                        MessageBox.Show(this, "El codigo debe ser un numero entero.", "Validacion",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    cliente = await _controlador.BuscarAsync(codigo);
                }
                else
                {
                    cliente = await _controlador.BuscarPorCedulaAsync(texto);
                }

                MostrarClientes(new[] { cliente });
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
            using (var dialogo = new FormClienteDialog(null))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    var respuesta = await _controlador.CrearAsync(dialogo.Cliente);
                    MessageBox.Show(this, respuesta.Mensaje, "Cliente creado",
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

        private async Task EditarAsync(Cliente cliente)
        {
            using (var dialogo = new FormClienteDialog(cliente))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    var respuesta = await _controlador.ActualizarAsync(dialogo.Cliente);
                    MessageBox.Show(this, respuesta.Mensaje, "Cliente actualizado",
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

        private async Task EliminarAsync(Cliente cliente)
        {
            var confirmacion = MessageBox.Show(
                this,
                "Eliminar al cliente " + cliente.Nombre + "?",
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
                await _controlador.EliminarAsync(cliente.CodCliente);
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

            var cliente = _grid.Rows[e.RowIndex].Tag as Cliente;
            if (cliente == null)
            {
                return;
            }

            var columna = _grid.Columns[e.ColumnIndex].Name;
            if (columna == "Editar")
            {
                await EditarAsync(cliente);
            }
            else if (columna == "Eliminar")
            {
                await EliminarAsync(cliente);
            }
        }
    }
}
