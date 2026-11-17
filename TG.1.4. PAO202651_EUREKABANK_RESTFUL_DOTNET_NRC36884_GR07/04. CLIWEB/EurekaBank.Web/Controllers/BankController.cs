using EurekaBank.Client;
using EurekaBank.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EurekaBank.Web.Controllers;

/// <summary>Controlador MVC: la vista web nunca consume HTTP de forma directa.</summary>
public sealed class BankController(IHttpClientFactory httpClientFactory) : Controller
{
    private EurekaBankRestClient Api => new(httpClientFactory.CreateClient("EurekaBankRest"));
    private string? Token => HttpContext.Session.GetString("Token");
    private string Role => HttpContext.Session.GetString("Role") ?? "CLIENTE";
    private bool IsStaff => Role is "ADMIN" or "CAJERO";
    private bool IsAjax => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

    public IActionResult Login() => Token is null ? View() : RedirectToAction(nameof(Panel));

    [HttpPost]
    public IActionResult Login(string userName, string password)
    {
        try
        {
            var cliente = Api;
            var resultado = cliente.Login(userName, password);
            if (!resultado.Ok || resultado.Data is null) { ViewBag.Error = resultado.Message; return View(); }
            HttpContext.Session.SetString("Token", resultado.Data.Token);
            HttpContext.Session.SetString("Name", resultado.Data.Name);
            HttpContext.Session.SetString("Role", resultado.Data.Role);
            return RedirectToAction(nameof(Panel));
        }
        catch (Exception ex) { ViewBag.Error = Limpiar(ex.Message); return View(); }
    }

    public IActionResult Panel(string? movementAccount)
    {
        if (Token is null) return RedirectToAction(nameof(Login));

        var panel = new BankPanel
        {
            Name = HttpContext.Session.GetString("Name") ?? "",
            Role = Role,
            SelectedMovementAccount = movementAccount ?? ""
        };

        try
        {
            var cliente = Api;
            var cuentas = cliente.Accounts(Token);
            panel.Accounts = cuentas.Data ?? [];
            panel.Error = cuentas.Ok ? null : cuentas.Message;

            var perfil = cliente.Profile(Token);
            panel.Profile = perfil.Data;

            if (IsStaff)
            {
                var clientes = cliente.Customers(Token);
                panel.Customers = clientes.Data ?? [];
                if (!clientes.Ok) panel.Error ??= clientes.Message;
            }

            if (!string.IsNullOrWhiteSpace(movementAccount))
            {
                var movimientos = cliente.Movements(Token, movementAccount);
                panel.Movements = MovimientosParaVista(movimientos.Data);
                panel.MovementMessage = movimientos.Ok ? null : movimientos.Message;
            }
        }
        catch (Exception ex) { panel.Error = Limpiar(ex.Message); }

        return View(panel);
    }

    [HttpPost]
    public IActionResult Transfer(string origin, string destination, string amount)
    {
        if (!TryAmount(amount, out var value, out var message)) return ValidationError(message);
        return Run(cliente => cliente.Transfer(Token!, origin, destination, value));
    }

    [HttpPost]
    public IActionResult Movement(string account, string type, string amount)
    {
        if (!IsStaff) return Denied();
        if (!TryAmount(amount, out var value, out var message)) return ValidationError(message);
        return Run(cliente => cliente.Movement(Token!, account, type, value));
    }

    [HttpPost]
    public IActionResult Reverse(string movementId)
    {
        if (!IsStaff) return Denied();
        return Run(cliente => cliente.Reverse(Token!, movementId));
    }

    [HttpPost]
    public IActionResult CreateAccount(string customerId)
    {
        if (!IsStaff) return Denied();
        return Run(cliente => cliente.CreateAccount(Token!, customerId));
    }

    [HttpPost]
    public IActionResult Movements(string account) =>
        QueryMovements(account);

    [HttpGet]
    public IActionResult AccountsData()
    {
        if (Token is null) return Json(new { ok = false, message = "La sesión ha expirado." });
        try
        {
            var cliente = Api;
            var resultado = cliente.Accounts(Token);
            return Json(new { ok = resultado.Ok, message = resultado.Message, data = resultado.Data ?? [] });
        }
        catch (Exception ex) { return Json(new { ok = false, message = Limpiar(ex.Message), data = Array.Empty<AccountDto>() }); }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        try { if (Token is not null) { Api.Logout(Token); } }
        catch { /* La sesión local siempre se cierra aunque el servicio no responda. */ }
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    private IActionResult Run<T>(Func<EurekaBankRestClient, ServiceResult<T>> operation)
    {
        if (Token is null) return SessionExpired();
        try
        {
            var cliente = Api;
            var resultado = operation(cliente);
            if (IsAjax) return Json(new { ok = resultado.Ok, message = resultado.Message, data = resultado.Data });
            TempData["Message"] = resultado.Message;
            TempData["MessageOk"] = resultado.Ok ? "true" : "false";
        }
        catch (Exception ex)
        {
            if (IsAjax) return Json(new { ok = false, message = Limpiar(ex.Message) });
            TempData["Message"] = Limpiar(ex.Message);
            TempData["MessageOk"] = "false";
        }
        return RedirectToAction(nameof(Panel));
    }

    private IActionResult Denied()
    {
        if (IsAjax) return Json(new { ok = false, message = "No tiene permiso para realizar esta operación." });
        TempData["Message"] = "No tiene permiso para realizar esta operación.";
        TempData["MessageOk"] = "false";
        return RedirectToAction(nameof(Panel));
    }

    private IActionResult ValidationError(string message)
    {
        if (IsAjax) return Json(new { ok = false, message });
        TempData["Message"] = message;
        TempData["MessageOk"] = "false";
        return RedirectToAction(nameof(Panel));
    }

    /// <summary>Admite 7,2 y 7.2, pero rechaza miles o formatos ambiguos antes de llamar a la API.</summary>
    private static bool TryAmount(string? input, out decimal value, out string message)
    {
        var text = (input ?? "").Trim();
        if (!Regex.IsMatch(text, @"^\d+(?:[,.]\d{1,2})?$"))
        {
            value = 0;
            message = "El monto debe tener hasta dos decimales, por ejemplo: 7,20 o 7.20.";
            return false;
        }

        // La API recibe decimal JSON con punto. Se normaliza para evitar que 7,2 se convierta en 72.
        var normalized = text.Replace(',', '.');
        if (!decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
        {
            message = "El monto no tiene un formato válido.";
            return false;
        }

        if (value <= 0)
        {
            message = "El monto debe ser mayor que cero.";
            return false;
        }

        message = "";
        return true;
    }

    private IActionResult QueryMovements(string account)
    {
        if (Token is null) return SessionExpired();
        try
        {
            var cliente = Api;
            var resultado = cliente.Movements(Token, account?.Trim() ?? "");
            if (IsAjax) return Json(new { ok = resultado.Ok, message = resultado.Message, data = MovimientosParaVista(resultado.Data) });
            return RedirectToAction(nameof(Panel), new { movementAccount = account?.Trim() ?? "" });
        }
        catch (Exception ex)
        {
            if (IsAjax) return Json(new { ok = false, message = Limpiar(ex.Message), data = Array.Empty<MovementDto>() });
            TempData["Message"] = Limpiar(ex.Message); TempData["MessageOk"] = "false";
            return RedirectToAction(nameof(Panel));
        }
    }

    private IActionResult SessionExpired()
    {
        if (IsAjax) return Json(new { ok = false, message = "La sesión ha expirado. Inicia sesión nuevamente." });
        return RedirectToAction(nameof(Login));
    }

    private static string Limpiar(string? message) => string.IsNullOrWhiteSpace(message) ? "No se pudo completar la operación." : message;

    // El código técnico se conserva en la API/BDD; esta traducción es solo para la interfaz web.
    private static List<MovementDto> MovimientosParaVista(IEnumerable<MovementDto>? movimientos) =>
        (movimientos ?? []).Select(m =>
        {
            m.Type = m.Type == "INGRESO" ? "DEPÓSITO" : m.Type;
            return m;
        }).ToList();
}

public sealed class BankPanel
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Error { get; set; }
    public string? MovementMessage { get; set; }
    public string SelectedMovementAccount { get; set; } = "";
    public List<AccountDto> Accounts { get; set; } = [];
    public List<MovementDto> Movements { get; set; } = [];
    public List<CustomerDto> Customers { get; set; } = [];
    public ProfileDto? Profile { get; set; }
}
