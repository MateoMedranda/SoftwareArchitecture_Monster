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
    public sealed class FormFacturas : Form
    {
        private readonly FacturaControlador _controlador;
        private readonly DataGridView _grid;
        private readonly TextBox _txtBusqueda;
        private readonly Label _lblEstado;

        public FormFacturas()
        {
            _controlador = new FacturaControlador();

            Text = "Celulares Monster - Facturas";
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
                Text = "Buscar por numero",
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

            var btnAgregar = Estilos.CrearBoton("+  Agregar factura", Estilos.Verde, 172, 36);
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

            var colCod = Estilos.ColumnaTexto("Cod", "COD", 70, true);
            var colFecha = Estilos.ColumnaTexto("Fecha", "Fecha de emision", 145, true);
            var colCedula = Estilos.ColumnaTexto("Cedula", "Cedula cliente", 180, true);
            var colValor = Estilos.ColumnaTexto("Valor", "Valor total", 135, true);
            var colTipo = Estilos.ColumnaTexto("Tipo", "Tipo", 120, true);
            colValor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colCod.FillWeight = 8;
            colFecha.FillWeight = 16;
            colCedula.FillWeight = 20;
            colValor.FillWeight = 15;
            colTipo.FillWeight = 14;

            _grid.Columns.Add(colCod);
            _grid.Columns.Add(colFecha);
            _grid.Columns.Add(colCedula);
            _grid.Columns.Add(colValor);
            _grid.Columns.Add(colTipo);

            var colDetalle = Estilos.ColumnaBoton("Detalle", "Ver detalle", 110);
            colDetalle.FillWeight = 14;
            _grid.Columns.Add(colDetalle);

            _grid.CellContentClick += GridCellContentClick;
            contenedorGrid.Controls.Add(_grid);

            _lblEstado = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(48, 0, 0, 0),
                ForeColor = Estilos.TextoSecundario,
                Font = new Font("Segoe UI", 8F),
                Text = "Cargando facturas..."
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

            var titulo = Estilos.CrearTitulo("Facturas");
            titulo.Location = new Point(300, 42);
            panel.Controls.Add(titulo);

            var subtitulo = new Label
            {
                Text = "Consulta las facturas emitidas",
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

            var btnFacturas = Estilos.CrearBotonNavegacion("Facturas", true);
            btnFacturas.Location = new Point(42, 7);
            panel.Controls.Add(btnFacturas);

            var btnClientes = Estilos.CrearBotonNavegacion("Clientes", false);
            btnClientes.Location = new Point(142, 7);
            btnClientes.Click += (sender, args) => Navegacion.Abrir(this, new FormClientes());
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

        private async Task CargarAsync()
        {
            try
            {
                UseWaitCursor = true;
                var facturas = await _controlador.ListarAsync();
                MostrarFacturas(facturas);
                _lblEstado.Text = facturas.Count + " factura(s) registrada(s).";
            }
            catch (Exception ex)
            {
                _lblEstado.Text = "No se pudo cargar la lista de facturas.";
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void MostrarFacturas(IEnumerable<Factura> facturas)
        {
            _grid.Rows.Clear();
            foreach (var factura in facturas)
            {
                var indice = _grid.Rows.Add(
                    factura.NumFactura,
                    FormatearFecha(factura.Fecha),
                    factura.Cedula,
                    "$" + factura.Valor.ToString("N2", CultureInfo.CurrentCulture),
                    FormatearTipo(factura.TipoPago),
                    "Ver detalle");
                _grid.Rows[indice].Tag = factura;
            }
        }

        private async Task AgregarAsync()
        {
            using (var dialogo = new FormAgregarFacturaDialog())
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            await CargarAsync();
        }

        private async Task BuscarAsync()
        {
            var texto = _txtBusqueda.Text.Trim();
            if (texto.Length == 0)
            {
                await CargarAsync();
                return;
            }

            int numero;
            if (!int.TryParse(texto, out numero))
            {
                MessageBox.Show(this, "El codigo de factura debe ser un numero entero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                var factura = await _controlador.BuscarAsync(numero);
                MostrarFacturas(new[] { factura });
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

        private static string FormatearFecha(string fecha)
        {
            DateTime valor;
            return DateTime.TryParse(fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor)
                ? valor.ToString("dd/MM/yyyy")
                : (fecha ?? string.Empty);
        }

        private static string FormatearTipo(string tipoPago)
        {
            var tipo = (tipoPago ?? string.Empty).Trim().ToUpperInvariant();
            if (tipo == "CREDITO" || tipo == "CREDITO_DIRECTO")
            {
                return "Credito";
            }

            if (tipo == "EFECTIVO")
            {
                return "Efectivo";
            }

            return tipoPago ?? string.Empty;
        }

        private async void GridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (_grid.Columns[e.ColumnIndex].Name != "Detalle")
            {
                return;
            }

            var factura = _grid.Rows[e.RowIndex].Tag as Factura;
            if (factura == null)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                var detalle = await _controlador.BuscarAsync(factura.NumFactura);
                if (detalle == null)
                {
                    throw new InvalidOperationException("El API no devolvio el detalle de la factura.");
                }

                using (var dialogo = new FormDetalleFacturaDialog(detalle))
                {
                    dialogo.ShowDialog(this);
                }
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
}
