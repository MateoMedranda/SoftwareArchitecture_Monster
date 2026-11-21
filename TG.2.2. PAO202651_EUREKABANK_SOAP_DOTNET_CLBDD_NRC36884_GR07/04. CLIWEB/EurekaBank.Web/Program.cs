var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(); builder.Services.AddSession();
var app = builder.Build(); app.UseStaticFiles(); app.UseRouting(); app.UseSession(); app.MapControllerRoute(name: "default", pattern: "{controller=Bank}/{action=Login}/{id?}"); app.Run();
