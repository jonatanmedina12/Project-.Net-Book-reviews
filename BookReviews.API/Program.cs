using BookReviews.API.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.ConfigurarSerilog();

// Agregar servicios a la aplicación
builder.Services.AgregarServiciosAplicacion(builder.Configuration);

// Agregar controladores
builder.Services.AddControllers();

builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Agregar OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configurar middleware
app.ConfigurarMiddleware();

app.UseStaticFiles();



app.Run();