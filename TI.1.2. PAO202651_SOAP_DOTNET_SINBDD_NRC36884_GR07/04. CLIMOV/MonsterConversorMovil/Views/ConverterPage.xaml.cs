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
        CategoryPicker.ItemsSource = controller.Categories.ToList();
        CategoryPicker.SelectedIndex = 0;
        GreetingLabel.Text = $"Hola, {controller.UserName}. Convierte con precisión.";
        UpdateCategory();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GreetingLabel.Text = $"Hola, {controller.UserName}. Convierte con precisión.";
    }

    private void OnCategoryChanged(object? sender, EventArgs eventArgs) => UpdateCategory();

    private void UpdateCategory()
    {
        var category = CategoryPicker.SelectedItem as CategoryOption;
        var units = controller.GetUnits(category).ToList();
        OriginPicker.ItemsSource = units;
        DestinationPicker.ItemsSource = units.ToList();
        OriginPicker.SelectedIndex = units.Count > 0 ? 0 : -1;
        DestinationPicker.SelectedIndex = units.Count > 1 ? 1 : OriginPicker.SelectedIndex;

        CategoryTitleLabel.Text = category?.Name ?? "Conversión";
        CategoryDescriptionLabel.Text = category?.Description ?? "Conectado al servicio SOAP";
        CategoryIconLabel.Text = category?.Icon ?? "↔";
        RangeLabel.Text = category?.Name == "Temperatura"
            ? "Rango físico del agua a 1 atm: el servicio valida cada entrada antes de convertirla."
            : "El servicio acepta valores positivos para esta categoría.";
        ResultCard.IsVisible = false;
        StatusLabel.IsVisible = false;
    }

    private async void OnConvertClicked(object? sender, EventArgs eventArgs)
    {
        SetBusy(true);
        ResultCard.IsVisible = false;
        StatusLabel.IsVisible = false;
        var result = await controller.ConvertAsync(
            ValueEntry.Text,
            CategoryPicker.SelectedItem as CategoryOption,
            OriginPicker.SelectedItem as UnitOption,
            DestinationPicker.SelectedItem as UnitOption);
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
