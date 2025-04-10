using BookReviews.API.Extensions;
using BookReviews.API.Utilities;
var builder = WebApplication.CreateBuilder(args);

// Primero cargar variables de entorno desde .env
DotEnv.Load();

// Luego agregar configuraci�n de variables de entorno
builder.Configuration.AddEnvironmentVariables();

// Imprimir variables importantes para debugging
Console.WriteLine("Variables de entorno despu�s de la carga:");
Console.WriteLine($"DEFAULT_CONNECTION presente: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION"))}");
Console.WriteLine($"JWT_SECRET presente: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET"))}");

// Configurar Serilog
builder.ConfigurarSerilog();

// A�adir servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar servicios de aplicaci�n
builder.Services.AgregarServiciosAplicacion(builder.Configuration);

var app = builder.Build();

// Configurar la canalizaci�n de solicitudes HTTP
app.ConfigurarMiddleware();
app.Run();