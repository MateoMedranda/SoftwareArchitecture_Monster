using System.Globalization;
using MonsterConversorMovil.Controllers;
using MonsterConversorMovil.Models;

namespace MonsterConversorMovil.Views;

public partial class ConverterPage : ContentPage
{
    private readonly ConversionController controller;

    public ConverterPage(ConversionController controller)
    {
        InitializeComponent();
        this.controller = controller;
		OriginPicker.ItemsSource = controller.TemperatureUnits.ToList();
		DestinationPicker.ItemsSource = controller.TemperatureUnits.ToList();
        OriginPicker.SelectedIndex = 0;
        DestinationPicker.SelectedIndex = 1;
        GreetingLabel.Text = $"Hola, {controller.UserName}. Convierte con precisión.";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GreetingLabel.Text = $"Hola, {controller.UserName}. Convierte con precisión.";
    }

    private async void OnConvertClicked(object? sender, EventArgs eventArgs)
    {
        SetBusy(true);
        ResultCard.IsVisible = false;
        StatusLabel.IsVisible = false;
        var result = await controller.ConvertTemperatureAsync(ValueEntry.Text, OriginPicker.SelectedItem as UnitOption, DestinationPicker.SelectedItem as UnitOption);
        SetBusy(false);

        if (!result.IsSuccessful)
        {
            ShowStatus(result.Message, false);
            return;
        }

        var unit = DestinationPicker.SelectedItem as UnitOption;
        ResultLabel.Text = $"{result.ConvertedValue.ToString("0.##", CultureInfo.CurrentCulture)} {unit?.Symbol}";
        ResultDetailLabel.Text = result.Message;
        ResultCard.IsVisible = true;
    }

    private async void OnSignOutClicked(object? sender, EventArgs eventArgs)
    {
        controller.SignOut();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private void SetBusy(bool isBusy)
    {
        ConvertButton.IsEnabled = !isBusy;
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
