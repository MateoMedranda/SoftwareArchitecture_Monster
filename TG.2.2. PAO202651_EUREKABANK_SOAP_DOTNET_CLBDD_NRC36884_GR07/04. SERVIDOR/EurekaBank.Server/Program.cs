using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using EurekaBank.Server;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cargar .development.env desde la raíz del proyecto (si existe).
EnvFileLoader.Load(Path.Combine(builder.Environment.ContentRootPath, ".development.env"));

// 2. Re-registrar el proveedor de variables de entorno: CreateBuilder ya cargó
//    los proveedores, así que hay que añadir uno nuevo para ver lo recién cargado.
builder.Configuration.AddEnvironmentVariables();

// 3. Cadena de conexión: override explícito > variables DB_* > error claro.
var connectionString = ConexionDB.Build(builder.Configuration);

builder.Services.AddDbContext<EurekaDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<EurekaBankSoapService>();
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<EurekaBankSoapService>();
    // Un endpoint por esquema: BasicHttpBinding no atiende HTTPS por sí solo.
    serviceBuilder.AddServiceEndpoint<EurekaBankSoapService, IEurekaBankSoapService>(new BasicHttpBinding(), "/EurekaBankSoap.svc");
    serviceBuilder.AddServiceEndpoint<EurekaBankSoapService, IEurekaBankSoapService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/EurekaBankSoap.svc");
    var metadata = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    metadata.HttpGetEnabled = true;
    metadata.HttpsGetEnabled = true;
});
app.MapGet("/", () => Results.Redirect("/EurekaBankSoap.svc?wsdl"));
app.Run();
