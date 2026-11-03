using System.Globalization;
using System.Text;
using EurekaBank.Console.Controladores;
using EurekaBank.Console.Vistas;
using EurekaBank.Contracts;

ConfigurarConsola();
using var controlador = new BancaControlador();

while (controlador.Sesion is null)
{
    System.Console.Clear();
    PresentacionConsola.MostrarMarca();
    PresentacionConsola.MostrarTitulo("Inicio de sesión · API RESTful");
    var usuario = PresentacionConsola.PedirTexto("Usuario");
    var clave = PresentacionConsola.PedirContrasena("Contraseña");
    if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(clave))
    {
        PresentacionConsola.Error("El usuario y la contraseña son obligatorios.");
        PresentacionConsola.EsperarTecla();
        continue;
    }

    try
    {
        var acceso = controlador.Ingresar(usuario, clave);
        if (!acceso.Ok || acceso.Data is null)
        {
            PresentacionConsola.Error(acceso.Message);
            PresentacionConsola.EsperarTecla();
        }
    }
    catch (Exception ex)
    {
        PresentacionConsola.Error($"No fue posible conectar a la API REST: {ex.Message}");
        PresentacionConsola.EsperarTecla();
    }
}

var sesion = controlador.Sesion!;
while (true)
{
    System.Console.Clear();
    PresentacionConsola.MostrarMarca();
    PresentacionConsola.Info($"Bienvenido, {sesion.Name} · {sesion.Role}");
    var acciones = CrearMenu(sesion.Role);
    var seleccion = new MenuSeleccionMultiple("Menú principal", acciones.Select(x => x.Texto).ToArray()).Mostrar();
    if (seleccion < 0) continue;
    var accion = acciones[seleccion].Codigo;
    if (accion == "SALIR") break;

    System.Console.Clear();
    PresentacionConsola.MostrarMarca();
    try { Ejecutar(accion); }
    catch (Exception ex) { PresentacionConsola.Error(ex.Message); }
    PresentacionConsola.EsperarTecla();
}

PresentacionConsola.Info("Sesión finalizada. Gracias por usar EurekaBank.");

void Ejecutar(string accion)
{
    switch (accion)
    {
        case "CUENTAS":
            PresentacionConsola.MostrarTitulo("Mis cuentas");
            MostrarCuentas(controlador.Cuentas());
            break;
        case "TRANSFERIR":
            PresentacionConsola.MostrarTitulo("Transferir dinero");
            var origen = PresentacionConsola.PedirTexto("Cuenta de origen");
            var destino = PresentacionConsola.PedirTexto("Cuenta de destino");
            if (PresentacionConsola.PedirMonto("Monto", out var monto)) MostrarMovimientos(controlador.Transferir(origen, destino, monto));
            break;
        case "MOVIMIENTOS":
            PresentacionConsola.MostrarTitulo("Movimientos de cuenta");
            MostrarMovimientos(controlador.Movimientos(PresentacionConsola.PedirTexto("Número de cuenta")));
            break;
        case "PERFIL":
            PresentacionConsola.MostrarTitulo("Mi perfil");
            MostrarPerfil(controlador.Perfil());
            break;
        case "OPERAR":
            PresentacionConsola.MostrarTitulo("Depósito o retiro");
            var cuenta = PresentacionConsola.PedirTexto("Número de cuenta");
            var tipoIngresado = PresentacionConsola.PedirTexto("Tipo (DEPÓSITO / RETIRO)").ToUpperInvariant();
            var tipo = tipoIngresado == "DEPÓSITO" || tipoIngresado == "DEPOSITO" ? "INGRESO" : tipoIngresado;
            if (PresentacionConsola.PedirMonto("Monto", out var montoOperacion)) MostrarMovimiento(controlador.Operar(cuenta, tipo, montoOperacion));
            break;
        case "REVERSAR":
            PresentacionConsola.MostrarTitulo("Reversar movimiento");
            PresentacionConsola.Info("Copie el ID completo mostrado en los movimientos.");
            MostrarMovimiento(controlador.Reversar(PresentacionConsola.PedirTexto("ID del movimiento")));
            break;
        case "CREAR":
            PresentacionConsola.MostrarTitulo("Crear cuenta");
            var clientes = controlador.Clientes();
            MostrarClientes(clientes);
            if (clientes.Ok && clientes.Data is { Count: > 0 }) MostrarCuenta(controlador.CrearCuenta(PresentacionConsola.PedirTexto("ID del cliente titular")));
            break;
    }
}

static List<(string Codigo, string Texto)> CrearMenu(string rol)
{
    var opciones = new List<(string, string)> { ("CUENTAS", "Mis cuentas"), ("TRANSFERIR", "Transferir dinero"), ("MOVIMIENTOS", "Consultar movimientos"), ("PERFIL", "Mi perfil") };
    if (rol != "CLIENTE") opciones.AddRange([("OPERAR", "Depósito o retiro"), ("REVERSAR", "Reversar movimiento"), ("CREAR", "Crear cuenta")]);
    opciones.Add(("SALIR", "Salir"));
    return opciones;
}

static void MostrarCuentas(ServiceResult<List<AccountDto>> resultado)
{
    if (!PresentacionConsola.Resultado(resultado) || resultado.Data is null) return;
    if (resultado.Data.Count == 0) { PresentacionConsola.Info("No existen cuentas para mostrar."); return; }
    PresentacionConsola.Tabla(["Número", "Saldo", "Estado", "Disponibilidad", "Sucursal"], resultado.Data.Select(x => new[] { x.Number, PresentacionConsola.Dinero(x.Balance), x.Status, x.Availability, x.Branch }));
}

static void MostrarCuenta(ServiceResult<AccountDto> resultado)
{
    if (PresentacionConsola.Resultado(resultado) && resultado.Data is not null) PresentacionConsola.Tabla(["Número", "Saldo", "Estado", "Sucursal"], [[resultado.Data.Number, PresentacionConsola.Dinero(resultado.Data.Balance), resultado.Data.Status, resultado.Data.Branch]]);
}

static void MostrarMovimientos(ServiceResult<List<MovementDto>> resultado)
{
    if (!PresentacionConsola.Resultado(resultado) || resultado.Data is null) return;
    if (resultado.Data.Count == 0) { PresentacionConsola.Info("La cuenta no registra movimientos todavía."); return; }
    PresentacionConsola.Tabla(["ID", "Fecha", "Tipo", "Monto", "Cuenta", "Usuario"], resultado.Data.Select(x => new[] { x.Id, x.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"), TipoVisible(x.Type), PresentacionConsola.Dinero(x.Amount), x.AccountNumber, x.CreatedBy }));
}

static void MostrarMovimiento(ServiceResult<MovementDto> resultado)
{
    if (PresentacionConsola.Resultado(resultado) && resultado.Data is not null) PresentacionConsola.Tabla(["ID", "Fecha", "Tipo", "Monto", "Cuenta", "Usuario"], [[resultado.Data.Id, resultado.Data.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"), TipoVisible(resultado.Data.Type), PresentacionConsola.Dinero(resultado.Data.Amount), resultado.Data.AccountNumber, resultado.Data.CreatedBy]]);
}

static string TipoVisible(string tipo) => tipo == "INGRESO" ? "DEPÓSITO" : tipo;

static void MostrarPerfil(ServiceResult<ProfileDto> resultado)
{
    if (PresentacionConsola.Resultado(resultado) && resultado.Data is not null) PresentacionConsola.Tabla(["Nombre", "Usuario", "Teléfono"], [[resultado.Data.Name, resultado.Data.UserName, resultado.Data.Phone]]);
}

static void MostrarClientes(ServiceResult<List<CustomerDto>> resultado)
{
    if (PresentacionConsola.Resultado(resultado) && resultado.Data is not null) PresentacionConsola.Tabla(["ID", "Nombre", "Usuario"], resultado.Data.Select(x => new[] { x.Id, x.Name, x.UserName }));
}

static void ConfigurarConsola()
{
    System.Console.OutputEncoding = Encoding.UTF8;
    System.Console.InputEncoding = Encoding.UTF8;
    System.Console.Title = "EurekaBank · Cliente de Consola";
}
