using System.Globalization;
using EurekaBank.Contracts;
using EurekaBank.Mobile.Controladores;

namespace EurekaBank.Mobile;

public partial class MainPage : ContentPage
{
    private readonly BancaControlador controlador = new();

    public MainPage()
    {
        InitializeComponent();
        EndpointEntry.Text = controlador.UrlServidor;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EndpointEntry.Text = controlador.UrlServidor;
    }

    private async void SignIn(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserName.Text) || string.IsNullOrWhiteSpace(Password.Text))
        {
            LoginStatus.Text = "Ingresa tu usuario y contraseña.";
            return;
        }

        SetLoginBusy(true);
        LoginStatus.Text = "Conectando con el servidor SOAP...";
        try
        {
            var result = await Task.Run(() => controlador.Ingresar(UserName.Text.Trim(), Password.Text));
            if (!result.Ok || result.Data is null)
            {
                LoginStatus.Text = result.Message;
                return;
            }

            Welcome.Text = $"Hola, {result.Data.Name}";
            LoginCard.IsVisible = false;
            Dashboard.IsVisible = true;
            StaffNavigation.IsVisible = controlador.EsPersonal;
            OperationsSection.IsVisible = false;
            AccountsSection.IsVisible = false;
            ShowSection("SUMMARY");
            await LoadSummaryAsync();
            if (controlador.EsPersonal) await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            LoginStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            SetLoginBusy(false);
        }
    }

    private async void Navigate(object sender, EventArgs e)
    {
        var section = (sender as Button)?.CommandParameter?.ToString() ?? "SUMMARY";
        ShowSection(section);

        if (section == "PROFILE") await LoadProfileAsync();
        if (section == "ACCOUNTS" && controlador.EsPersonal) await LoadCustomersAsync();
    }

    private void ShowSection(string section)
    {
        SummarySection.IsVisible = section == "SUMMARY";
        MovementsSection.IsVisible = section == "MOVEMENTS";
        TransferSection.IsVisible = section == "TRANSFER";
        OperationsSection.IsVisible = section == "OPERATIONS" && controlador.EsPersonal;
        AccountsSection.IsVisible = section == "ACCOUNTS" && controlador.EsPersonal;
        ProfileSection.IsVisible = section == "PROFILE";
    }

    private async Task LoadSummaryAsync()
    {
        SummaryStatus.Text = "Actualizando cuentas...";
        try
        {
            var result = await Task.Run(controlador.Cuentas);
            Accounts.ItemsSource = result.Data ?? [];
            SummaryStatus.Text = result.Message;
        }
        catch (Exception ex)
        {
            SummaryStatus.Text = MensajeConexion(ex);
        }
    }

    private async void RefreshAccounts(object sender, EventArgs e) => await LoadSummaryAsync();

    private async void QueryMovements(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MovementAccount.Text))
        {
            MovementStatus.Text = "Ingresa el número de cuenta.";
            return;
        }

        MovementButton.IsEnabled = false;
        MovementStatus.Text = "Consultando movimientos...";
        try
        {
            var result = await Task.Run(() => controlador.Movimientos(MovementAccount.Text.Trim()));
            Movements.ItemsSource = result.Data ?? [];
            MovementStatus.Text = result.Message;
        }
        catch (Exception ex)
        {
            MovementStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            MovementButton.IsEnabled = true;
        }
    }

    private async void Transfer(object sender, EventArgs e)
    {
        if (!TryAmount(Amount.Text, out var amount))
        {
            TransferStatus.Text = "Ingresa un monto válido, por ejemplo 7,20.";
            return;
        }

        TransferButton.IsEnabled = false;
        TransferStatus.Text = "Enviando transferencia...";
        try
        {
            var result = await Task.Run(() => controlador.Transferir(Origin.Text ?? "", Destination.Text ?? "", amount));
            TransferStatus.Text = result.Message;
            if (result.Ok) await LoadSummaryAsync();
        }
        catch (Exception ex)
        {
            TransferStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            TransferButton.IsEnabled = true;
        }
    }

    private async void RegisterCashOperation(object sender, EventArgs e)
    {
        if (!TryAmount(CashAmount.Text, out var amount))
        {
            CashStatus.Text = "Ingresa un monto válido.";
            return;
        }

        var type = CashType.SelectedIndex == 1 ? "RETIRO" : "INGRESO";
        CashButton.IsEnabled = false;
        CashStatus.Text = "Registrando operación...";
        try
        {
            var result = await Task.Run(() => controlador.Operar(CashAccount.Text ?? "", type, amount));
            CashStatus.Text = result.Message;
            if (result.Ok) await LoadSummaryAsync();
        }
        catch (Exception ex)
        {
            CashStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            CashButton.IsEnabled = true;
        }
    }

    private async void ReverseMovement(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ReverseId.Text))
        {
            ReverseStatus.Text = "Ingresa el ID del movimiento.";
            return;
        }

        ReverseButton.IsEnabled = false;
        ReverseStatus.Text = "Registrando reversa...";
        try
        {
            var result = await Task.Run(() => controlador.Reversar(ReverseId.Text.Trim()));
            ReverseStatus.Text = result.Message;
            if (result.Ok) await LoadSummaryAsync();
        }
        catch (Exception ex)
        {
            ReverseStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            ReverseButton.IsEnabled = true;
        }
    }

    private async Task LoadCustomersAsync()
    {
        if (!controlador.EsPersonal) return;
        try
        {
            var result = await Task.Run(controlador.Clientes);
            CustomerPicker.ItemsSource = result.Data ?? [];
            AccountsStatus.Text = result.Ok ? result.Message : result.Message;
        }
        catch (Exception ex)
        {
            AccountsStatus.Text = MensajeConexion(ex);
        }
    }

    private async void CreateAccount(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedItem is not CustomerDto customer)
        {
            AccountsStatus.Text = "Selecciona el cliente titular.";
            return;
        }

        CreateAccountButton.IsEnabled = false;
        AccountsStatus.Text = "Creando cuenta...";
        try
        {
            var result = await Task.Run(() => controlador.CrearCuenta(customer.Id));
            AccountsStatus.Text = result.Message;
            if (result.Ok) await LoadSummaryAsync();
        }
        catch (Exception ex)
        {
            AccountsStatus.Text = MensajeConexion(ex);
        }
        finally
        {
            CreateAccountButton.IsEnabled = true;
        }
    }

    private async Task LoadProfileAsync()
    {
        ProfileStatus.Text = "Consultando perfil...";
        try
        {
            var result = await Task.Run(controlador.Perfil);
            if (!result.Ok || result.Data is null)
            {
                ProfileStatus.Text = result.Message;
                return;
            }

            ProfileName.Text = result.Data.Name;
            ProfileUser.Text = $"Usuario: {result.Data.UserName}";
            ProfilePhone.Text = $"Teléfono: {result.Data.Phone}";
            ProfileAccounts.Text = $"Cuentas asociadas: {result.Data.Accounts.Count}";
            ProfileStatus.Text = result.Message;
        }
        catch (Exception ex)
        {
            ProfileStatus.Text = MensajeConexion(ex);
        }
    }

    private void ShowServerSettings(object sender, EventArgs e)
    {
        ServerSettings.IsVisible = !ServerSettings.IsVisible;
        EndpointEntry.Text = controlador.UrlServidor;
        ServerStatus.Text = string.Empty;
    }

    private void SaveServerSettings(object sender, EventArgs e)
    {
        if (!controlador.CambiarServidor(EndpointEntry.Text ?? string.Empty, out var endpoint, out var error))
        {
            ServerStatus.Text = error;
            return;
        }

        EndpointEntry.Text = endpoint;
        ServerStatus.Text = "Servidor guardado.";
        LoginStatus.Text = string.Empty;
    }

    private void CancelServerSettings(object sender, EventArgs e)
    {
        ServerSettings.IsVisible = false;
        EndpointEntry.Text = controlador.UrlServidor;
        ServerStatus.Text = string.Empty;
    }

    private async void SignOut(object sender, EventArgs e)
    {
        LoginCard.IsVisible = true;
        Dashboard.IsVisible = false;
        Password.Text = string.Empty;
        LoginStatus.Text = string.Empty;
        try { await Task.Run(controlador.Salir); }
        catch { /* El cierre local no depende de la red. */ }
    }

    private void SetLoginBusy(bool busy)
    {
        LoginBusy.IsRunning = busy;
        LoginBusy.IsVisible = busy;
        LoginButton.IsEnabled = !busy;
        ServerButton.IsEnabled = !busy;
    }

    private static bool TryAmount(string? raw, out decimal amount) =>
        decimal.TryParse((raw ?? string.Empty).Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out amount) && amount > 0;

    private static string MensajeConexion(Exception exception) =>
        exception is TimeoutException
            ? "El servidor no respondió a tiempo. Revisa la URL configurada."
            : $"No se pudo comunicar con el servidor SOAP. {exception.Message}";
}
