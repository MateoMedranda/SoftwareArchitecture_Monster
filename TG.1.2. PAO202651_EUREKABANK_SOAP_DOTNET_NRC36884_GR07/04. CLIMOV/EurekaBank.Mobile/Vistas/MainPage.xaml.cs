using EurekaBank.Mobile.Controladores;
namespace EurekaBank.Mobile;
public partial class MainPage : ContentPage
{
 private readonly BancaControlador controlador = new();
 public MainPage() => InitializeComponent();
 private void SignIn(object sender, EventArgs e) { var r=controlador.Ingresar(UserName.Text??"",Password.Text??""); if(!r.Ok||r.Data is null){LoginStatus.Text=r.Message;return;} Welcome.Text=$"Hola, {r.Data.Name}"; LoginCard.IsVisible=false; Dashboard.IsVisible=true; LoadAccounts(); }
 private void LoadAccounts(){var r=controlador.Cuentas();Accounts.ItemsSource=r.Data;Status.Text=r.Message;}
 private void Transfer(object sender,EventArgs e){if(!decimal.TryParse(Amount.Text,out var amount)){Status.Text="Monto inválido.";return;}var r=controlador.Transferir(Origin.Text??"",Destination.Text??"",amount);Status.Text=r.Message;if(r.Ok)LoadAccounts();}
 private void SignOut(object sender,EventArgs e){controlador.Salir();LoginCard.IsVisible=true;Dashboard.IsVisible=false;Password.Text="";}
}
