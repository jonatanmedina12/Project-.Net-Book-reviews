using BookReviews.API.Extensions;
using BookReviews.API.Utilities;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

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

