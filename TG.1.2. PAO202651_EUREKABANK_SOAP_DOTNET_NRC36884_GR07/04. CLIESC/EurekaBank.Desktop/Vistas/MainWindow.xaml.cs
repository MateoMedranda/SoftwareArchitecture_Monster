using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EurekaBank.Contracts;
using EurekaBank.Desktop.Controladores;

namespace EurekaBank.Desktop;

/// <summary>Vista WPF: delega todas las operaciones SOAP al controlador MVC.</summary>
public partial class MainWindow : Window
{
    private readonly BancaControlador controlador = new();
    private readonly CultureInfo cultura = CultureInfo.GetCultureInfo("es-EC");

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => UserName.Focus();
    }

    private async void SignIn(object sender, RoutedEventArgs e)
    {
        var usuario = UserName.Text.Trim();
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(Password.Password))
        {
            LoginMessage.Text = "Ingresa usuario y contraseña.";
            return;
        }

        LoginButton.IsEnabled = false;
        LoginMessage.Foreground = Brushes.SteelBlue;
        LoginMessage.Text = "Conectando con EurekaBank...";
        try
        {
            var resultado = await Task.Run(() => controlador.Ingresar(usuario, Password.Password));
            if (!resultado.Ok || resultado.Data is null)
            {
                LoginMessage.Foreground = Brushes.IndianRed;
                LoginMessage.Text = resultado.Message;
                return;
            }

            var sesion = resultado.Data;
            WelcomeText.Text = $"Hola, {sesion.Name}";
            RoleText.Text = sesion.Role;
            var esCliente = string.Equals(sesion.Role, "CLIENTE", StringComparison.OrdinalIgnoreCase);
            StaffOperationsNav.Visibility = esCliente ? Visibility.Collapsed : Visibility.Visible;
            StaffAccountsNav.Visibility = esCliente ? Visibility.Collapsed : Visibility.Visible;
            LoginView.Visibility = Visibility.Collapsed;
            ApplicationView.Visibility = Visibility.Visible;
            SummaryNav.IsChecked = true;
            MostrarPagina("Summary");
            MostrarEstado("Sesión iniciada correctamente.", true);
            await CargarCuentasAsync();
        }
        catch (Exception ex)
        {
            LoginMessage.Foreground = Brushes.IndianRed;
            LoginMessage.Text = Limpiar(ex.Message);
        }
        finally { LoginButton.IsEnabled = true; }
    }

    private void Password_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) SignIn(sender, e);
    }

    private async void Navigate(object sender, RoutedEventArgs e)
    {
        if (controlador.Sesion is null || sender is not RadioButton { IsChecked: true } boton) return;
        var pagina = boton.Tag?.ToString() ?? "Summary";
        MostrarPagina(pagina);
        if (pagina == "Profile") await CargarPerfilAsync();
        if (pagina == "Accounts") await CargarClientesAsync();
    }

    private void MostrarPagina(string pagina)
    {
        SummaryPage.Visibility = pagina == "Summary" ? Visibility.Visible : Visibility.Collapsed;
        MovementsPage.Visibility = pagina == "Movements" ? Visibility.Visible : Visibility.Collapsed;
        TransferPage.Visibility = pagina == "Transfer" ? Visibility.Visible : Visibility.Collapsed;
        OperationsPage.Visibility = pagina == "Operations" ? Visibility.Visible : Visibility.Collapsed;
        AccountsPage.Visibility = pagina == "Accounts" ? Visibility.Visible : Visibility.Collapsed;
        ProfilePage.Visibility = pagina == "Profile" ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void RefreshAccounts(object sender, RoutedEventArgs e) => await CargarCuentasAsync();

    private async Task CargarCuentasAsync()
    {
        try
        {
            var resultado = await Task.Run(controlador.Cuentas);
            if (!resultado.Ok || resultado.Data is null) { MostrarEstado(resultado.Message, false); return; }
            AccountsGrid.ItemsSource = resultado.Data;
            TotalBalanceText.Text = $"USD {resultado.Data.Sum(x => x.Balance):N2}";
            AccountCountText.Text = $"{resultado.Data.Count} cuenta{(resultado.Data.Count == 1 ? "" : "s")}";
            MostrarEstado(resultado.Message, true);
        }
        catch (Exception ex) { MostrarEstado(Limpiar(ex.Message), false); }
    }

    private void AccountSelected(object sender, SelectionChangedEventArgs e)
    {
        if (AccountsGrid.SelectedItem is not AccountDto cuenta) return;
        MovementAccountBox.Text = cuenta.Number;
        TransferOriginBox.Text = cuenta.Number;
        CashAccountBox.Text = cuenta.Number;
    }

    private async void LoadMovements(object sender, RoutedEventArgs e)
    {
        var cuenta = MovementAccountBox.Text.Trim();
        if (!ValidarCuenta(cuenta, MovementStatus)) return;
        MovementStatus.Foreground = Brushes.SteelBlue;
        MovementStatus.Text = "Consultando historial...";
        try
        {
            var resultado = await Task.Run(() => controlador.Movimientos(cuenta));
            if (!resultado.Ok || resultado.Data is null) { Mensaje(MovementStatus, resultado.Message, false); return; }
            MovementsGrid.ItemsSource = resultado.Data;
            Mensaje(MovementStatus, resultado.Data.Count == 0 ? "La cuenta no registra movimientos todavía." : $"{resultado.Data.Count} movimiento(s) encontrado(s).", true);
        }
        catch (Exception ex) { Mensaje(MovementStatus, Limpiar(ex.Message), false); }
    }

    private async void Transfer(object sender, RoutedEventArgs e)
    {
        var origen = TransferOriginBox.Text.Trim();
        var destino = TransferDestinationBox.Text.Trim();
        if (!ValidarCuenta(origen, TransferStatus, "La cuenta de origen") || !ValidarCuenta(destino, TransferStatus, "La cuenta de destino")) return;
        if (origen == destino) { Mensaje(TransferStatus, "Las cuentas de origen y destino deben ser diferentes.", false); return; }
        if (!TryMonto(TransferAmountBox.Text, out var monto)) { Mensaje(TransferStatus, "Ingresa un monto mayor que cero.", false); return; }

        try
        {
            Mensaje(TransferStatus, "Procesando transferencia...", true);
            var resultado = await Task.Run(() => controlador.Transferir(origen, destino, monto));
            if (!resultado.Ok) { Mensaje(TransferStatus, resultado.Message, false); return; }
            TransferOriginBox.Clear(); TransferDestinationBox.Clear(); TransferAmountBox.Clear();
            Mensaje(TransferStatus, resultado.Message, true);
            MostrarEstado("Transferencia completada y auditada.", true);
            await CargarCuentasAsync();
        }
        catch (Exception ex) { Mensaje(TransferStatus, Limpiar(ex.Message), false); }
    }

    private async void RegisterCashOperation(object sender, RoutedEventArgs e)
    {
        var cuenta = CashAccountBox.Text.Trim();
        if (!ValidarCuenta(cuenta, CashStatus, "El número de cuenta")) return;
        if (!TryMonto(CashAmountBox.Text, out var monto)) { Mensaje(CashStatus, "Ingresa un monto mayor que cero.", false); return; }
        var tipo = (CashTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "INGRESO";
        try
        {
            Mensaje(CashStatus, "Registrando operación...", true);
            var resultado = await Task.Run(() => controlador.Operar(cuenta, tipo, monto));
            if (!resultado.Ok) { Mensaje(CashStatus, resultado.Message, false); return; }
            CashAmountBox.Clear();
            Mensaje(CashStatus, resultado.Message, true);
            MostrarEstado($"{tipo} registrado correctamente.", true);
            await CargarCuentasAsync();
        }
        catch (Exception ex) { Mensaje(CashStatus, Limpiar(ex.Message), false); }
    }

    private async void ReverseMovement(object sender, RoutedEventArgs e)
    {
        var id = ReverseIdBox.Text.Trim();
        if (!Guid.TryParse(id, out _)) { Mensaje(ReverseStatus, "Ingresa un ID de movimiento válido.", false); return; }
        try
        {
            Mensaje(ReverseStatus, "Registrando reversa...", true);
            var resultado = await Task.Run(() => controlador.Reversar(id));
            if (!resultado.Ok) { Mensaje(ReverseStatus, resultado.Message, false); return; }
            ReverseIdBox.Clear();
            Mensaje(ReverseStatus, resultado.Message, true);
            MostrarEstado("Reversa registrada correctamente.", true);
            await CargarCuentasAsync();
        }
        catch (Exception ex) { Mensaje(ReverseStatus, Limpiar(ex.Message), false); }
    }

    private async Task CargarClientesAsync()
    {
        try
        {
            var resultado = await Task.Run(controlador.Clientes);
            if (!resultado.Ok || resultado.Data is null) { Mensaje(CreateAccountStatus, resultado.Message, false); return; }
            CustomerBox.ItemsSource = resultado.Data;
            Mensaje(CreateAccountStatus, resultado.Data.Count == 0 ? "No existen clientes disponibles." : "Selecciona el titular para crear la cuenta.", resultado.Data.Count > 0);
        }
        catch (Exception ex) { Mensaje(CreateAccountStatus, Limpiar(ex.Message), false); }
    }

    private async void CreateAccount(object sender, RoutedEventArgs e)
    {
        if (CustomerBox.SelectedItem is not CustomerDto cliente) { Mensaje(CreateAccountStatus, "Selecciona un cliente antes de continuar.", false); return; }
        try
        {
            var resultado = await Task.Run(() => controlador.CrearCuenta(cliente.Id));
            if (!resultado.Ok || resultado.Data is null) { Mensaje(CreateAccountStatus, resultado.Message, false); return; }
            CustomerBox.SelectedIndex = -1;
            Mensaje(CreateAccountStatus, $"Cuenta {resultado.Data.Number} creada y asignada a {cliente.Name}.", true);
            MostrarEstado("Nueva cuenta creada correctamente.", true);
            await CargarCuentasAsync();
        }
        catch (Exception ex) { Mensaje(CreateAccountStatus, Limpiar(ex.Message), false); }
    }

    private async Task CargarPerfilAsync()
    {
        try
        {
            var resultado = await Task.Run(controlador.Perfil);
            if (!resultado.Ok || resultado.Data is null) { MostrarEstado(resultado.Message, false); return; }
            ProfileNameText.Text = resultado.Data.Name;
            ProfileUserText.Text = $"@{resultado.Data.UserName}";
            ProfilePhoneText.Text = resultado.Data.Phone;
            ProfileAccountsText.Text = $"{resultado.Data.Accounts.Count} cuenta{(resultado.Data.Accounts.Count == 1 ? "" : "s")} asociada(s)";
        }
        catch (Exception ex) { MostrarEstado(Limpiar(ex.Message), false); }
    }

    private async void SignOut(object sender, RoutedEventArgs e)
    {
        try { await Task.Run(controlador.Salir); } catch { /* La vista se puede cerrar incluso si el servidor ya no responde. */ }
        ApplicationView.Visibility = Visibility.Collapsed;
        LoginView.Visibility = Visibility.Visible;
        Password.Clear(); LoginMessage.Text = "Sesión cerrada correctamente."; LoginMessage.Foreground = Brushes.SeaGreen;
        UserName.Focus();
    }

    private bool ValidarCuenta(string cuenta, TextBlock destino, string etiqueta = "El número de cuenta")
    {
        if (cuenta.Length == 10 && cuenta.All(char.IsDigit)) return true;
        Mensaje(destino, $"{etiqueta} debe tener exactamente 10 dígitos.", false);
        return false;
    }

    private bool TryMonto(string texto, out decimal monto) =>
        (decimal.TryParse(texto, NumberStyles.Number, cultura, out monto) || decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out monto)) && monto > 0;

    private void Mensaje(TextBlock destino, string texto, bool exito)
    {
        destino.Foreground = exito ? Brushes.SeaGreen : Brushes.IndianRed;
        destino.Text = texto;
    }

    private void MostrarEstado(string texto, bool exito)
    {
        GlobalStatus.Foreground = exito ? Brushes.SeaGreen : Brushes.IndianRed;
        GlobalStatus.Text = texto;
    }

    private static string Limpiar(string? mensaje) => string.IsNullOrWhiteSpace(mensaje) ? "No se pudo completar la operación." : mensaje;

    protected override void OnClosed(EventArgs e)
    {
        try { controlador.Dispose(); } catch { }
        base.OnClosed(e);
    }
}
