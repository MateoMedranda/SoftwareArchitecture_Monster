using EurekaBank.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<EurekaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EurekaBank")));
builder.Services.AddScoped<EurekaBankService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EurekaBank REST API",
        Version = "v1",
        Description = "API REST versionada de EurekaBank para las operaciones de banca electrónica."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Token",
        Description = "Escriba el token obtenido en POST /api/v1/auth/sessions."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EurekaBank REST API v1");
    options.RoutePrefix = "swagger";
});
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();
