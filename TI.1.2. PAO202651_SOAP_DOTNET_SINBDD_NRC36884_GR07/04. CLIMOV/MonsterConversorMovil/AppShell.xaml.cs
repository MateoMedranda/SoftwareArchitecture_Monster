using Microsoft.Extensions.DependencyInjection;
using MonsterConversorMovil.Views;

namespace MonsterConversorMovil;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Route = "LoginPage",
            Content = services.GetRequiredService<LoginPage>()
        });
        Items.Add(new ShellContent
        {
            Route = "ConverterPage",
            Content = services.GetRequiredService<ConverterPage>()
        });
    }
}
