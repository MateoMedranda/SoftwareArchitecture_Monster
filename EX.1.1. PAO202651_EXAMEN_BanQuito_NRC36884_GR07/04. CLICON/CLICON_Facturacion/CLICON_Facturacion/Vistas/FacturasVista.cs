using System;
using System.Threading.Tasks;
using CLICON_Facturacion.Controladores;
using CLICON_Facturacion.Modelos;
using CLICON_Facturacion.Servicios;

namespace CLICON_Facturacion.Vistas
{
    /// <summary>
    /// Vistas de consulta de facturas y su tabla de amortización.
    /// La creación de facturas queda pendiente por decisión del alcance.
    /// </summary>
    public sealed class FacturasVista
    {
        private const string HexBanquitoAzul = "#666BED";
        private const string HexBanquitoRojo = "#F96468";

        private readonly FacturaControlador _controlador;
        private readonly ClienteControlador _clientes;
        private readonly ProductoControlador _productos;
        private readonly AmortizacionControlador _amortizacion;

        public FacturasVista()
        {
            _controlador = new FacturaControlador();
            _clientes = new ClienteControlador();
            _productos = new ProductoControlador();
            _amortizacion = new AmortizacionControlador();
        }

        public async Task MostrarAsync()
        {
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Facturas");
                Consola.MenuOpcion(1, "Nueva factura");
                Consola.MenuOpcion(2, "Ver lista de facturas");
                Consola.MenuOpcion(3, "Buscar factura");
                Consola.MenuOpcion(4, "Atrás");

                var opcion = Consola.LeerOpcion("   Seleccione una opción: ", 1, 4);
                switch (opcion)
                {
                    case 1:
                        await NuevaFacturaAsync();
                        break;
                    case 2:
                        await ListarAsync();
                        break;
                    case 3:
                        await BuscarAsync();
                        break;
                    case 4:
                        return;
                }
            }
        }

        private async Task NuevaFacturaAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Facturas / Nueva factura");

            // 1. Validar Cliente
            Cliente cliente = null;
            while (cliente == null)
            {
                var cedula = Consola.LeerTexto("   Cédula del cliente (0 para volver): ", true, 10, null, true);
                if (cedula == "0" || cedula == null) return;

                try
                {
                    cliente = await _clientes.BuscarPorCedulaAsync(cedula);
                    Consola.Info("   Cliente: " + cliente.Nombre);
                }
                catch (Exception ex)
                {
                    Consola.MostrarError(ex);
                }
            }

            // 2. Carrito de compras
            var carrito = new System.Collections.Generic.List<CarritoItem>();
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Facturas / Nueva factura - Agregar productos");
                Consola.Info("   Cliente: " + cliente.Nombre);
                Console.WriteLine();

                if (carrito.Count > 0)
                {
                    MostrarCarrito(carrito);
                }

                var codProd = Consola.LeerEntero("   Código de producto (0 para terminar): ", 1, int.MaxValue, true);
                if (codProd == null || codProd == 0)
                {
                    if (carrito.Count == 0) return;
                    break;
                }

                try
                {
                    var producto = await _productos.BuscarAsync(codProd.Value);
                    var cantidad = Consola.LeerEntero("   Cantidad: ", 1, 999, false);
                    if (cantidad != null)
                    {
                        carrito.Add(new CarritoItem { Producto = producto, Cantidad = cantidad.Value });
                    }
                }
                catch (Exception ex)
                {
                    Consola.MostrarError(ex);
                    Consola.Pausa();
                }

                if (carrito.Count > 0)
                {
                    Consola.Limpiar();
                    Consola.Encabezado("Facturas / Nueva factura - Carrito");
                    MostrarCarrito(carrito);
                    Consola.MenuOpcion(1, "Agregar otro ítem");
                    Consola.MenuOpcion(2, "Terminar compra");
                    Consola.MenuOpcion(3, "Cancelar y salir");

                    var nav = Consola.LeerOpcion("   Seleccione una opción: ", 1, 3);
                    if (nav == 3) return;
                    if (nav == 2) break;
                }
            }

            // 3. Método de Pago
            Consola.Limpiar();
            Consola.Encabezado("Facturas / Nueva factura - Pago");
            MostrarCarrito(carrito);
            
            Consola.MenuOpcion(1, "Pago en efectivo (42% de descuento)");
            Console.Write("   [2] Pago a Crédito - ");
            Consola.Escribir("BAN", ConsoleColor.Blue, HexBanquitoAzul);
            Consola.EscribirLinea("QUITO", ConsoleColor.Red, HexBanquitoRojo);
            Consola.MenuOpcion(3, "Cancelar");

            var metodo = Consola.LeerOpcion("   Seleccione método de pago: ", 1, 3);
            if (metodo == 3) return;

            string tipoPago = metodo == 1 ? "EFECTIVO" : "CREDITO";
            int? cuotas = null;
            decimal totalOriginal = 0;
            foreach (var i in carrito) totalOriginal += i.Subtotal;

            if (metodo == 1)
            {
                var totalDescuento = Math.Round(totalOriginal * 0.58m, 2, MidpointRounding.AwayFromZero);
                Consola.Aviso("   Pago en efectivo");
                Consola.Info("   El total a pagar es de " + Consola.Moneda(totalDescuento) + " con el descuento del 42%");
                if (!Consola.Confirmar("   ¿Confirmar pago?")) return;
            }
            else
            {
                cuotas = Consola.LeerEntero("   Número de cuotas (3-18): ", 3, 18, false);
                if (cuotas == null) return;
                
                Consola.Aviso("   Crédito Directo BanQuito");
                Consola.Info("   Valor factura: " + Consola.Moneda(totalOriginal));
                Consola.Info("   Cuotas: " + cuotas);
                if (!Consola.Confirmar("   ¿Confirmar solicitud de crédito?")) return;
            }

            // 4. Enviar
            try
            {
                var itemsReq = new System.Collections.Generic.List<FacturaItemRequest>();
                foreach (var ci in carrito) itemsReq.Add(new FacturaItemRequest { NumInventario = ci.Producto.NumInventario, Cantidad = ci.Cantidad });

                var factura = await _controlador.CrearAsync(cliente.CodCliente, tipoPago, cuotas, itemsReq);
                Consola.Exito("   Factura N° " + factura.NumFactura + " generada exitosamente.");

                if (tipoPago == "CREDITO")
                {
                    Consola.Pausa("   Presione Enter para ver la tabla de amortización...");
                    var am = await _amortizacion.ObtenerPorFacturaAsync(factura.NumFactura);
                    Consola.Limpiar();
                    Consola.Encabezado("Facturas / Crédito Emitido");
                    MostrarAmortizacion(am);
                    Consola.Pausa("   Presione Enter para cerrar y volver al menú.");
                }
                else
                {
                    Consola.Pausa();
                }
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
                Consola.Pausa();
            }
        }

        private void MostrarCarrito(System.Collections.Generic.List<CarritoItem> carrito)
        {
            var tabla = new TablaConsola("Código", "Producto", "Cantidad", "Precio", "Subtotal");
            decimal total = 0;
            foreach (var item in carrito)
            {
                tabla.Agregar(item.Producto.NumInventario, item.Producto.Nombre, item.Cantidad, Consola.Moneda(item.Producto.Precio), Consola.Moneda(item.Subtotal));
                total += item.Subtotal;
            }
            tabla.Imprimir();
            Consola.Info("   TOTAL: " + Consola.Moneda(total));
            Console.WriteLine();
        }

        private sealed class CarritoItem
        {
            public Producto Producto { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal { get { return Producto.Precio * Cantidad; } }
        }

        private async Task ListarAsync()
        {
            Consola.Limpiar();
            Consola.Encabezado("Facturas / Lista de facturas");

            try
            {
                var facturas = await _controlador.ListarAsync();
                if (facturas == null || facturas.Count == 0)
                {
                    Consola.Info("No hay facturas registradas.");
                }
                else
                {
                    var tabla = new TablaConsola(
                        "N° Factura",
                        "Fecha",
                        "Cliente",
                        "Cédula",
                        "Tipo pago",
                        "Valor",
                        "N° crédito");

                    foreach (var factura in facturas)
                    {
                        tabla.Agregar(
                            factura.NumFactura,
                            Consola.Fecha(factura.Fecha),
                            factura.NombreCliente,
                            factura.Cedula,
                            factura.TipoPago,
                            Consola.Moneda(factura.Valor),
                            factura.NumCredito.HasValue ? factura.NumCredito.Value.ToString() : "-");
                    }

                    tabla.Imprimir();
                    Consola.Info("Total de facturas: " + facturas.Count);
                }
            }
            catch (Exception ex)
            {
                Consola.MostrarError(ex);
            }

            Consola.Pausa();
        }

        private async Task BuscarAsync()
        {
            while (true)
            {
                Consola.Limpiar();
                Consola.Encabezado("Facturas / Buscar factura");

                var numero = Consola.LeerEntero(
                    "   Código de factura (0 para volver): ",
                    1,
                    int.MaxValue,
                    true);

                if (!numero.HasValue)
                {
                    return;
                }

                try
                {
                    var factura = await _controlador.BuscarAsync(numero.Value);
                    await MostrarDetalleAsync(factura);
                    Consola.Pausa();
                    return;
                }
                catch (ApiException ex) when (ex.StatusCode == 404)
                {
                    Consola.Error("[Error] Factura no encontrada. Ingrese otro código.");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Consola.MostrarError(ex);
                    Consola.Pausa();
                    return;
                }
            }
        }

        private async Task MostrarDetalleAsync(Factura factura)
        {
            Consola.Limpiar();
            Consola.Encabezado("Facturas / Detalle de factura");

            var tabla = new TablaConsola(
                "N° Factura",
                "Fecha",
                "Cliente",
                "Cédula",
                "Tipo pago",
                "Valor",
                "N° crédito");

            tabla.Agregar(
                factura.NumFactura,
                Consola.Fecha(factura.Fecha),
                factura.NombreCliente,
                factura.Cedula,
                factura.TipoPago,
                Consola.Moneda(factura.Valor),
                factura.NumCredito.HasValue ? factura.NumCredito.Value.ToString() : "-");

            tabla.Imprimir();
            Console.WriteLine();

            Consola.Info("Ítems de la factura");
            if (factura.Items == null || factura.Items.Count == 0)
            {
                Consola.Tenue("   La factura no tiene ítems registrados.");
            }
            else
            {
                var items = new TablaConsola("Código", "Producto", "Cantidad", "Precio", "Subtotal");
                foreach (var item in factura.Items)
                {
                    items.Agregar(
                        item.NumInventario,
                        item.NombreProducto,
                        item.Cantidad,
                        Consola.Moneda(item.Valor),
                        Consola.Moneda(item.Valor * item.Cantidad));
                }

                items.Imprimir();
            }

            Console.WriteLine();

            if (!factura.NumCredito.HasValue)
            {
                Consola.Info("Esta factura fue pagada en efectivo y no tiene crédito asociado.");
                return;
            }

            try
            {
                var amortizacion = await _amortizacion.ObtenerPorFacturaAsync(factura.NumFactura);
                MostrarAmortizacion(amortizacion);
            }
            catch (Exception ex)
            {
                Consola.Aviso("No se pudo obtener la tabla de amortización: " + Consola.MensajeDeError(ex));
            }
        }

        private static void MostrarAmortizacion(RespuestaAmortizacion amortizacion)
        {
            if (amortizacion == null)
            {
                Consola.Aviso("El API no devolvió información de amortización.");
                return;
            }

            Consola.Info("Tabla de amortización - crédito N° " + amortizacion.NumCredito);
            if (amortizacion.Cuotas == null || amortizacion.Cuotas.Count == 0)
            {
                Consola.Tenue("   El crédito no tiene cuotas registradas.");
                return;
            }

            var tabla = new TablaConsola(
                "# Cuota",
                "Valor cuota",
                "Interés",
                "Capital",
                "Saldo",
                "Fecha pago",
                "Pagada");

            foreach (var cuota in amortizacion.Cuotas)
            {
                tabla.Agregar(
                    cuota.NumCuota,
                    Consola.Moneda(cuota.ValorCuota),
                    Consola.Moneda(cuota.InteresPagado),
                    Consola.Moneda(cuota.CapitalPagado),
                    Consola.Moneda(cuota.Saldo),
                    Consola.Fecha(cuota.FechaPago),
                    Consola.SiNo(cuota.Pagada));
            }

            tabla.Imprimir();
        }
    }
}
