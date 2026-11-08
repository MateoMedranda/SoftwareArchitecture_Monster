using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ServidorConversionesDotnet.Controladores;
using ServidorConversionesDotnet.Filtros;
using ServidorConversionesDotnet.Modelos;
using ServidorConversionesDotnet.Servicios;
using ServidorConversionesDotnet.Utilidades;

var configurador = WebApplication.CreateBuilder(args);

// 1. Cargar las variables del archivo .development.env (tienen prioridad
//    sobre los valores de appsettings.json).
var variablesDesarrollo = CargadorEnv.Cargar(configurador.Environment.ContentRootPath);
configurador.Configuration.AddInMemoryCollection(variablesDesarrollo);

// 2. Configurar la dirección del servidor con URL_BASE / PUERTO del .env.
var urlBase = configurador.Configuration["URL_BASE"];
var puerto = configurador.Configuration["PUERTO"];

if (!string.IsNullOrWhiteSpace(urlBase))
{
    configurador.WebHost.UseUrls(urlBase);
}
else if (!string.IsNullOrWhiteSpace(puerto))
{
    configurador.WebHost.UseUrls($"http://localhost:{puerto}");
}

if (string.IsNullOrWhiteSpace(configurador.Configuration["CLAVE_SECRETA"]))
{
    throw new InvalidOperationException(
        "Falta la variable CLAVE_SECRETA en el archivo .development.env.");
}

// 3. Servicios de la aplicación.
configurador.Services.AddSingleton<GeneradorToken>();
configurador.Services.AddSingleton<ServicioAutenticacion>();
configurador.Services.AddSingleton<ServicioConversionMasa>();
configurador.Services.AddSingleton<ServicioConversionLongitud>();
configurador.Services.AddSingleton<ServicioConversionTemperatura>();

// 4. Controladores con el filtro de autorización de token global
//    (el login se exenta con [AllowAnonymous]).
configurador.Services.AddControllers(options =>
{
    options.Filters.Add<FiltroAutorizacionToken>();
});

// Documentación interactiva de las rutas REST y configuración del token JWT.
configurador.Services.AddEndpointsApiExplorer();
configurador.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Conversión de Unidades",
        Version = "v1",
        Description = "API REST para autenticar usuarios y convertir unidades de longitud, masa y temperatura."
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue el token JWT obtenido en /api/autenticacion/login."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 5. Todas las respuestas de validación usan el formato RespuestaError.
configurador.Services.Configure<ApiBehaviorOptions>(opciones =>
{
    opciones.InvalidModelStateResponseFactory = contexto =>
    {
        var error = new RespuestaError
        {
            Codigo = StatusCodes.Status400BadRequest,
            Mensaje = "Debe ingresar el valor, la unidad de origen y la unidad de destino.",
            Detalle = "El cuerpo de la solicitud no contiene valores válidos o está incompleto."
        };

        return new BadRequestObjectResult(error);
    };
});

var aplicacion = configurador.Build();

aplicacion.UseSwagger();
aplicacion.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Conversión de Unidades v1");
    opciones.DocumentTitle = "Documentación API de Conversión";
});

aplicacion.MapControllers();

aplicacion.Run();
