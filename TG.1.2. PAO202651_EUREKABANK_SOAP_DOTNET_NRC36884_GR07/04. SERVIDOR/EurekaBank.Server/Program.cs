using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using EurekaBank.Server;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EurekaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EurekaBank")));
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
