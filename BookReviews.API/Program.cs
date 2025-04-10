using BookReviews.API.Extensions;
using BookReviews.API.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Primero, verificar explícitamente las variables de entorno de Railway
Console.WriteLine("Verificando variables de entorno de Railway:");
Console.WriteLine($"DEFAULT_CONNECTION presente: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION"))}");
Console.WriteLine($"DIRECT_CONNECTION presente: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIRECT_CONNECTION"))}");
Console.WriteLine($"JWT_SECRET presente: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET"))}");
Console.WriteLine($"JWT_EXPIRY_MINUTES: {Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES")}");

// Añadir variables de entorno primero
builder.Configuration.AddEnvironmentVariables();

// Cargar variables desde .env si estamos en desarrollo
DotEnv.Load();
// Configurar Serilog
builder.ConfigurarSerilog();

// Añadir servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar servicios de aplicación con el flag de migraciones
builder.Services.AgregarServiciosAplicacion(builder.Configuration);

var app = builder.Build();

// Configurar la canalización de solicitudes HTTP
app.ConfigurarMiddleware();

app.Run();

