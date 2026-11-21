var builder = WebApplication.CreateBuilder(args);
var restUrl = Environment.GetEnvironmentVariable("EUREKABANK_REST_URL") ?? "http://localhost:62558/api/v1";

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpClient("EurekaBankRest", client =>
{
    client.BaseAddress = new Uri(restUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Bank}/{action=Login}/{id?}");
app.Run();
