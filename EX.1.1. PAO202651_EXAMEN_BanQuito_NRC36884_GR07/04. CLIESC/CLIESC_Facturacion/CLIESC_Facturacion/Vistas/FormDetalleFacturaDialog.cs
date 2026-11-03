using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLIESC_Facturacion.Controladores;
using CLIESC_Facturacion.Modelos;

namespace CLIESC_Facturacion.Vistas
{
    public sealed class FormDetalleFacturaDialog : Form
    {
        private readonly Factura _factura;
        private readonly AmortizacionControlador _amortizacionControlador;
        private readonly ProductoControlador _productoControlador;
        private readonly DataGridView _gridItems;

        public FormDetalleFacturaDialog(Factura factura)
        {
            _factura = factura ?? new Factura();
            if (_factura.Items == null)
            {
                _factura.Items = new System.Collections.Generic.List<FacturaItem>();
            }

            _amortizacionControlador = new AmortizacionControlador();
            _productoControlador = new ProductoControlador();

            Text = "Detalle de Factura";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            Size = new Size(620, 690);
            BackColor = Estilos.Fondo;
            Font = new Font("Segoe UI", 9F);

            var esCredito = EsCredito(_factura.TipoPago);
            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Estilos.Fondo,
                ColumnCount = 1,
                RowCount = esCredito ? 5 : 4,
                Padding = new Padding(24, 12, 24, 14)
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 142));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            if (esCredito)
            {
                raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            }
            Controls.Add(raiz);

            var titulo = new Label
            {
                Text = "Factura N° " + _factura.NumFactura,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(titulo, 0, 0);

            raiz.Controls.Add(CrearResumen(_factura), 0, 1);

            var lblItems = new Label
            {
                Text = "Items de la factura",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Estilos.Texto
            };
            raiz.Controls.Add(lblItems, 0, 2);

            _gridItems = new DataGridView { Dock = DockStyle.Fill };
            Estilos.ConfigurarGrid(_gridItems);
            var colCodigo = Estilos.ColumnaTexto("Codigo", "Cod", 70, true);
            var colItem = Estilos.ColumnaTexto("Item", "Item", 220, true);
            var colPrecio = Estilos.ColumnaTexto("Precio", "Precio", 100, true);
            var colCantidad = Estilos.ColumnaTexto("Cantidad", "Cant", 70, true);
            var colSubtotal = Estilos.ColumnaTexto("Subtotal", "Subtotal", 110, true);
            colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCantidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSubtotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCodigo.FillWeight = 10;
            colItem.FillWeight = 40;
            colPrecio.FillWeight = 15;
            colCantidad.FillWeight = 12;
            colSubtotal.FillWeight = 18;
            _gridItems.Columns.Add(colCodigo);
            _gridItems.Columns.Add(colItem);
            _gridItems.Columns.Add(colPrecio);
            _gridItems.Columns.Add(colCantidad);
            _gridItems.Columns.Add(colSubtotal);

            var hayNombresPendientes = false;
            foreach (var item in _factura.Items)
            {
                if (string.IsNullOrWhiteSpace(item.NombreProducto))
                {
                    hayNombresPendientes = true;
                }

                _gridItems.Rows.Add(
                    item.NumInventario,
                    item.NombreProducto,
                    "$" + item.Valor.ToString("N2", CultureInfo.CurrentCulture),
                    item.Cantidad,
                    "$" + (item.Valor * item.Cantidad).ToString("N2", CultureInfo.CurrentCulture));
            }

            raiz.Controls.Add(_gridItems, 0, 3);

            if (esCredito)
            {
                var panelAmortizacion = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 1,
                    Padding = new Padding(0, 8, 0, 0)
                };
                var btnAmortizacion = Estilos.CrearBoton("Ver tabla de amortizacion", Estilos.Azul, 240, 38);
                btnAmortizacion.Anchor = AnchorStyles.None;
                btnAmortizacion.Click += async (sender, args) => await VerAmortizacionAsync();
                panelAmortizacion.Controls.Add(btnAmortizacion, 0, 0);
                raiz.Controls.Add(panelAmortizacion, 0, 4);
            }

            if (hayNombresPendientes)
            {
                Shown += async (sender, args) => await CompletarNombresAsync();
            }
        }

        private async Task VerAmortizacionAsync()
        {
            try
            {
                UseWaitCursor = true;
                var amortizacion = await _amortizacionControlador.ObtenerPorFacturaAsync(_factura.NumFactura);
                if (amortizacion == null)
                {
                    throw new InvalidOperationException("El API no devolvio la tabla de amortizacion.");
                }

                using (var dialogo = new FormTablaAmortizacionDialog(_factura, amortizacion))
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

        private async Task CompletarNombresAsync()
        {
            for (var i = 0; i < _factura.Items.Count; i++)
            {
                var item = _factura.Items[i];
                if (!string.IsNullOrWhiteSpace(item.NombreProducto))
                {
                    continue;
                }

                try
                {
                    var producto = await _productoControlador.BuscarAsync(item.NumInventario);
                    if (producto != null)
                    {
                        item.NombreProducto = producto.Nombre;
                        if (i < _gridItems.Rows.Count)
                        {
                            _gridItems.Rows[i].Cells["Item"].Value = producto.Nombre;
                        }
                    }
                }
                catch (Exception)
                {
                    // Se conserva el nombre vacio si el producto no se puede consultar.
                }
            }
        }

        private static TableLayoutPanel CrearResumen(Factura factura)
        {
            var resumen = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Estilos.Fondo
            };
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            resumen.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            resumen.Controls.Add(CrearEtiqueta("Fecha de emision"), 0, 0);
            resumen.Controls.Add(CrearEtiqueta("Total"), 1, 0);
            resumen.Controls.Add(CrearValor(FormatearFecha(factura.Fecha)), 0, 1);
            resumen.Controls.Add(CrearValor("$" + factura.Valor.ToString("N2", CultureInfo.CurrentCulture)), 1, 1);

            resumen.Controls.Add(CrearEtiqueta("Cedula del cliente"), 0, 2);
            resumen.Controls.Add(CrearEtiqueta("Tipo de pago"), 1, 2);
            resumen.Controls.Add(CrearValor(factura.Cedula), 0, 3);
            resumen.Controls.Add(CrearValor(FormatearTipoPago(factura.TipoPago)), 1, 3);

            return resumen;
        }

        private static Label CrearEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                ForeColor = Estilos.TextoSecundario,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 8.5F)
            };
        }

        private static Label CrearValor(string texto)
        {
            return new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                ForeColor = Estilos.Texto,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
        }

        private static bool EsCredito(string tipoPago)
        {
            var tipo = (tipoPago ?? string.Empty).Trim().ToUpperInvariant();
            return tipo == "CREDITO" || tipo == "CREDITO_DIRECTO";
        }

        private static string FormatearTipoPago(string tipoPago)
        {
            return EsCredito(tipoPago) ? "Credito" : "Efectivo";
        }

        private static string FormatearFecha(string fecha)
        {
            DateTime valor;
            return DateTime.TryParse(fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor)
                ? valor.ToString("dd/MM/yyyy")
                : (fecha ?? string.Empty);
        }
    }
}
