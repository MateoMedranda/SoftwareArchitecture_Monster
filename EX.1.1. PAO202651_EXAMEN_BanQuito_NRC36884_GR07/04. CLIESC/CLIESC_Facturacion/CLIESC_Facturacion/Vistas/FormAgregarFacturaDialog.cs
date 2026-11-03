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
    public sealed class FormAgregarFacturaDialog : Form
    {
        private static readonly Color Rojo = Color.FromArgb(239, 68, 68);

        private readonly FacturaControlador _facturaControlador;
        private readonly ClienteControlador _clienteControlador;
        private readonly ProductoControlador _productoControlador;
        private readonly AmortizacionControlador _amortizacionControlador;
        private readonly List<LineaFactura> _items;

        private Cliente _cliente;

        private TextBox _txtCedula;
        private Button _btnVerificar;
        private TextBox _txtCliente;
        private TextBox _txtCodigoProducto;
        private NumericUpDown _numCantidad;
        private Button _btnAgregarItem;
        private DataGridView _gridItems;
        private Label _lblTotal;
        private Button _btnEfectivo;
        private Button _btnCredito;

        public FormAgregarFacturaDialog()
        {
            _facturaControlador = new FacturaControlador();
            _clienteControlador = new ClienteControlador();
            _productoControlador = new ProductoControlador();
            _amortizacionControlador = new AmortizacionControlador();
            _items = new List<LineaFactura>();

            Text = "Registrar Compra";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(660, 760);
            MinimumSize = new Size(660, 760);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(28, 10, 28, 10)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            Controls.Add(raiz);
            FormClosing += (sender, args) => LimpiarMemoria();

            var titulo = new Label
            {
                Text = "Registrar Compra",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            raiz.Controls.Add(CrearPanelCliente(), 0, 1);
            raiz.Controls.Add(CrearPanelProducto(), 0, 2);

            var contenedorGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 6, 0, 6),
                BackColor = Estilos.Fondo
            };
            raiz.Controls.Add(contenedorGrid, 0, 3);

            _gridItems = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(_gridItems);
            var colCodigo = Estilos.ColumnaTexto("Codigo", "Cod", 70, true);
            var colItem = Estilos.ColumnaTexto("Item", "Item", 220, true);
            var colPrecio = Estilos.ColumnaTexto("Precio", "Precio", 90, true);
            var colCantidad = Estilos.ColumnaTexto("Cantidad", "Cant", 60, true);
            var colSubtotal = Estilos.ColumnaTexto("Subtotal", "Subtotal", 100, true);
            colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCantidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSubtotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCodigo.FillWeight = 10;
            colItem.FillWeight = 35;
            colPrecio.FillWeight = 15;
            colCantidad.FillWeight = 10;
            colSubtotal.FillWeight = 15;
            _gridItems.Columns.Add(colCodigo);
            _gridItems.Columns.Add(colItem);
            _gridItems.Columns.Add(colPrecio);
            _gridItems.Columns.Add(colCantidad);
            _gridItems.Columns.Add(colSubtotal);
            var colQuitar = Estilos.ColumnaBoton("Quitar", "Quitar", 80);
            colQuitar.FillWeight = 15;
            _gridItems.Columns.Add(colQuitar);
            _gridItems.CellContentClick += GridItemsCellContentClick;
            contenedorGrid.Controls.Add(_gridItems);

            raiz.Controls.Add(CrearPanelTotal(), 0, 4);
            raiz.Controls.Add(CrearPanelPago(), 0, 5);
            raiz.Controls.Add(CrearPanelCancelar(), 0, 6);

            EstablecerBloqueo(true);
            ActualizarTotal();
        }

        private Panel CrearPanelCliente()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var lblCedula = new Label
            {
                Text = "Cedula del cliente:",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panel.Controls.Add(lblCedula);

            _txtCedula = new TextBox
            {
                Location = new Point(0, 22),
                Size = new Size(240, 26),
                MaxLength = 10
            };
            _txtCedula.KeyDown += async (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    args.SuppressKeyPress = true;
                    await VerificarClienteAsync();
                }
            };
            _txtCedula.TextChanged += (sender, args) =>
            {
                if (_cliente != null)
                {
                    ReiniciarCliente();
                }
            };
            panel.Controls.Add(_txtCedula);

            _btnVerificar = Estilos.CrearBoton("Verificar", Estilos.Azul, 110, 28);
            _btnVerificar.Location = new Point(250, 21);
            _btnVerificar.Click += async (sender, args) => await VerificarClienteAsync();
            panel.Controls.Add(_btnVerificar);

            var lblCliente = new Label
            {
                Text = "Cliente",
                ForeColor = Estilos.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 54)
            };
            panel.Controls.Add(lblCliente);

            _txtCliente = new TextBox
            {
                Location = new Point(0, 76),
                Size = new Size(568, 26),
                ReadOnly = true,
                BackColor = Color.White,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            panel.Controls.Add(_txtCliente);

            return panel;
        }

        private Panel CrearPanelProducto()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var lblCodigo = new Label
            {
                Text = "Agregar item",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            panel.Controls.Add(lblCodigo);

            _txtCodigoProducto = new TextBox
            {
                Location = new Point(0, 22),
                Size = new Size(180, 26)
            };
            _txtCodigoProducto.KeyDown += async (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    args.SuppressKeyPress = true;
                    await AgregarItemAsync();
                }
            };
            panel.Controls.Add(_txtCodigoProducto);

            var lblCantidad = new Label
            {
                Text = "Cantidad",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Location = new Point(200, 0)
            };
            panel.Controls.Add(lblCantidad);

            _numCantidad = new NumericUpDown
            {
                Location = new Point(200, 22),
                Size = new Size(90, 26),
                Minimum = 1,
                Maximum = 999,
                Value = 1,
                Increment = 1,
                TextAlign = HorizontalAlignment.Right
            };
            panel.Controls.Add(_numCantidad);

            _btnAgregarItem = Estilos.CrearBoton("Agregar", Estilos.Texto, 110, 28);
            _btnAgregarItem.Location = new Point(310, 21);
            _btnAgregarItem.Click += async (sender, args) => await AgregarItemAsync();
            panel.Controls.Add(_btnAgregarItem);

            return panel;
        }

        private Panel CrearPanelTotal()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var lblTitulo = new Label
            {
                Text = "Total:",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                Location = new Point(0, 0)
            };
            panel.Controls.Add(lblTitulo);

            _lblTotal = new Label
            {
                Text = "$0.00",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Regular),
                Location = new Point(0, 27)
            };
            panel.Controls.Add(_lblTotal);

            return panel;
        }

        private Panel CrearPanelPago()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Estilos.Fondo };

            var lblMetodo = new Label
            {
                Text = "Metodo de Pago",
                ForeColor = Estilos.Texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(0, 0)
            };
            panel.Controls.Add(lblMetodo);

            _btnEfectivo = Estilos.CrearBoton("$  Pago en Efectivo", Estilos.Verde, 210, 42);
            _btnEfectivo.Location = new Point(10, 42);
            _btnEfectivo.Click += async (sender, args) => await PagarEfectivoAsync();
            panel.Controls.Add(_btnEfectivo);

            var logoBanquito = Estilos.CrearIcono(110, 30, "logo_banquito.jpg");
            logoBanquito.Location = new Point(390, 8);
            panel.Controls.Add(logoBanquito);

            _btnCredito = Estilos.CrearBoton("Credito Directo", Estilos.Azul, 210, 42);
            _btnCredito.Location = new Point(340, 42);
            _btnCredito.Click += async (sender, args) => await PagarCreditoAsync();
            panel.Controls.Add(_btnCredito);

            return panel;
        }

        private Panel CrearPanelCancelar()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                BackColor = Estilos.Fondo
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var btnCancelar = Estilos.CrearBoton("Cancelar", Rojo, 150, 38);
            btnCancelar.Anchor = AnchorStyles.None;
            btnCancelar.Click += CancelarClick;
            panel.Controls.Add(btnCancelar, 0, 0);
            CancelButton = btnCancelar;

            return panel;
        }

        private async Task VerificarClienteAsync()
        {
            var cedula = _txtCedula.Text.Trim();
            if (cedula.Length != 10)
            {
                MessageBox.Show(this, "La cedula debe tener exactamente 10 caracteres.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCedula.Focus();
                return;
            }

            try
            {
                UseWaitCursor = true;
                _btnVerificar.Enabled = false;
                var cliente = await _clienteControlador.BuscarPorCedulaAsync(cedula);
                if (cliente == null)
                {
                    throw new InvalidOperationException("No se encontro el cliente indicado.");
                }

                _cliente = cliente;
                _txtCliente.Text = cliente.Nombre;
                EstablecerBloqueo(false);
            }
            catch (Exception ex)
            {
                ReiniciarCliente();
                Estilos.MostrarError(this, ex);
            }
            finally
            {
                _btnVerificar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async Task AgregarItemAsync()
        {
            if (_cliente == null)
            {
                return;
            }

            var textoCodigo = _txtCodigoProducto.Text.Trim();
            int codigo;
            if (!int.TryParse(textoCodigo, out codigo))
            {
                MessageBox.Show(this, "El codigo del producto debe ser un numero entero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCodigoProducto.Focus();
                return;
            }

            var cantidad = (int)_numCantidad.Value;
            if (cantidad <= 0)
            {
                MessageBox.Show(this, "La cantidad debe ser mayor que cero.", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _numCantidad.Focus();
                return;
            }

            try
            {
                UseWaitCursor = true;
                var producto = await _productoControlador.BuscarAsync(codigo);
                if (producto == null)
                {
                    throw new InvalidOperationException("No se encontro el producto indicado.");
                }

                _items.Add(new LineaFactura
                {
                    NumInventario = producto.NumInventario,
                    NombreProducto = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });

                MostrarItems();
                _txtCodigoProducto.Clear();
                _numCantidad.Value = 1;
                _txtCodigoProducto.Focus();
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

        private void GridItemsCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (_gridItems.Columns[e.ColumnIndex].Name != "Quitar")
            {
                return;
            }

            var linea = _gridItems.Rows[e.RowIndex].Tag as LineaFactura;
            if (linea == null)
            {
                return;
            }

            _items.Remove(linea);
            MostrarItems();
        }

        private async Task PagarEfectivoAsync()
        {
            if (_cliente == null || _items.Count == 0)
            {
                return;
            }

            var total = ObtenerTotal();
            var totalConDescuento = Math.Round(total * 0.58m, 2, MidpointRounding.AwayFromZero);

            using (var dialogo = new FormConfirmarEfectivoDialog(totalConDescuento))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            await CrearFacturaAsync("EFECTIVO", null);
        }

        private async Task PagarCreditoAsync()
        {
            if (_cliente == null || _items.Count == 0)
            {
                return;
            }

            var total = ObtenerTotal();
            int numCuotas;

            using (var dialogo = new FormConfirmarCreditoDialog(total))
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                numCuotas = dialogo.NumCuotas;
            }

            await CrearFacturaAsync("CREDITO", numCuotas);
        }

        private async Task CrearFacturaAsync(string tipoPago, int? numCuotas)
        {
            if (_cliente == null || _items.Count == 0)
            {
                return;
            }

            var total = ObtenerTotal();
            var items = new List<FacturaItemRequest>();
            foreach (var linea in _items)
            {
                items.Add(new FacturaItemRequest
                {
                    NumInventario = linea.NumInventario,
                    Cantidad = linea.Cantidad
                });
            }

            Factura factura;
            try
            {
                UseWaitCursor = true;
                factura = await _facturaControlador.CrearAsync(
                    _cliente.CodCliente,
                    tipoPago,
                    numCuotas,
                    items);
                if (factura == null)
                {
                    throw new InvalidOperationException("El API no devolvio la factura creada.");
                }
            }
            catch (Exception ex)
            {
                Estilos.MostrarError(this, ex);
                return;
            }
            finally
            {
                UseWaitCursor = false;
            }

            if (tipoPago == "EFECTIVO")
            {
                MessageBox.Show(this,
                    "Factura N° " + factura.NumFactura + " creada correctamente.",
                    "Factura creada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                LimpiarMemoria();
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            RespuestaAmortizacion amortizacion;
            try
            {
                UseWaitCursor = true;
                amortizacion = await _amortizacionControlador.ObtenerPorFacturaAsync(factura.NumFactura);
                if (amortizacion == null)
                {
                    throw new InvalidOperationException("El API no devolvio la tabla de amortizacion.");
                }
            }
            catch (Exception ex)
            {
                Estilos.MostrarError(this, ex);
                MessageBox.Show(this,
                    "La factura N° " + factura.NumFactura +
                    " se creo correctamente, pero no se pudo consultar su tabla de amortizacion.",
                    "Factura creada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                LimpiarMemoria();
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            finally
            {
                UseWaitCursor = false;
            }

            using (var dialogo = new FormCreditoExitosoDialog(
                amortizacion,
                _cliente.CodCliente,
                numCuotas ?? 0,
                total))
            {
                dialogo.ShowDialog(this);
            }

            LimpiarMemoria();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelarClick(object sender, EventArgs e)
        {
            LimpiarMemoria();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EstablecerBloqueo(bool bloqueado)
        {
            _txtCodigoProducto.Enabled = !bloqueado;
            _numCantidad.Enabled = !bloqueado;
            _btnAgregarItem.Enabled = !bloqueado;
            _gridItems.Enabled = !bloqueado;
            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            var total = ObtenerTotal();
            _lblTotal.Text = "$" + total.ToString("N2", CultureInfo.CurrentCulture);

            var puedePagar = _cliente != null && _items.Count > 0;
            _btnEfectivo.Enabled = puedePagar;
            _btnCredito.Enabled = puedePagar;
        }

        private decimal ObtenerTotal()
        {
            var total = 0m;
            foreach (var linea in _items)
            {
                total += linea.Subtotal;
            }

            return total;
        }

        private void MostrarItems()
        {
            _gridItems.Rows.Clear();
            foreach (var linea in _items)
            {
                var indice = _gridItems.Rows.Add(
                    linea.NumInventario,
                    linea.NombreProducto,
                    "$" + linea.Precio.ToString("N2", CultureInfo.CurrentCulture),
                    linea.Cantidad,
                    "$" + linea.Subtotal.ToString("N2", CultureInfo.CurrentCulture),
                    "Quitar");
                _gridItems.Rows[indice].Tag = linea;
            }

            ActualizarTotal();
        }

        private void ReiniciarCliente()
        {
            _cliente = null;
            _txtCliente.Text = string.Empty;
            _items.Clear();
            MostrarItems();
            EstablecerBloqueo(true);
        }

        private void LimpiarMemoria()
        {
            _items.Clear();
            _cliente = null;
        }

        private sealed class LineaFactura
        {
            public int NumInventario { get; set; }
            public string NombreProducto { get; set; }
            public decimal Precio { get; set; }
            public int Cantidad { get; set; }

            public decimal Subtotal
            {
                get { return Precio * Cantidad; }
            }
        }
    }
}
