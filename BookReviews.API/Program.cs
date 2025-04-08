using BookReviews.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.ConfigurarSerilog();

// Agregar servicios a la aplicación
builder.Services.AgregarServiciosAplicacion(builder.Configuration);

// Agregar controladores
builder.Services.AddControllers();

// Agregar OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configurar middleware
app.ConfigurarMiddleware();

app.Run();