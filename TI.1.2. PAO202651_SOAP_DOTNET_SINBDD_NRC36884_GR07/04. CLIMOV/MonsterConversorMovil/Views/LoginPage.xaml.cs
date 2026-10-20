using MonsterConversorMovil.Controllers;

namespace MonsterConversorMovil.Views;

public partial class LoginPage : ContentPage
{
    private readonly AccountController controller;

    public LoginPage(AccountController controller)
    {
        InitializeComponent();
        this.controller = controller;
        EndpointEntry.Text = controller.Endpoint;
    }

    private async void OnSignInClicked(object? sender, EventArgs eventArgs)
    {
        SetBusy(true);
        controller.Endpoint = EndpointEntry.Text ?? string.Empty;
        var result = await controller.SignInAsync(UserNameEntry.Text ?? string.Empty, PasswordEntry.Text ?? string.Empty);
        SetBusy(false);

        if (result.IsSuccessful)
        {
            await Shell.Current.GoToAsync("//ConverterPage");
            PasswordEntry.Text = string.Empty;
            return;
        }

        ShowStatus(result.Message, false);
    }

    private void SetBusy(bool isBusy)
    {
        SignInButton.IsEnabled = !isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
    }

    private void ShowStatus(string message, bool success)
    {
        StatusLabel.Text = message;
        StatusLabel.TextColor = Color.FromArgb(success ? "#16835D" : "#C62828");
        StatusLabel.IsVisible = true;
    }
}
