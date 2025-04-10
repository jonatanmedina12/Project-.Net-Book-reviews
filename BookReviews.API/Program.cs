using BookReviews.API.Extensions;
using BookReviews.API.Utilities;
using System;

// Mostrar información de inicio
Console.WriteLine("Iniciando BookReviews API...");
Console.WriteLine($"Fecha y hora: {DateTime.Now}");
Console.WriteLine($"Directorio base: {AppDomain.CurrentDomain.BaseDirectory}");

try
{
    // Intentar cargar variables desde .env si el archivo existe
    Console.WriteLine("Intentando cargar archivo .env (opcional)...");
    DotEnv.Load();
}
catch (Exception ex)
{
    Console.WriteLine($"Error al cargar archivo .env (no crítico): {ex.Message}");
    Console.WriteLine("Continuando con valores por defecto o variables de entorno del sistema.");
}

try
{
    // Crear el builder de la aplicación
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder = builder.ConfigurarSerilog();

    // Obtener la configuración
    var configuration = builder.Configuration;

    // Aplicar los valores por defecto si no hay variables de entorno
    configuration.CargarVariablesEntorno();

    // Mostrar aplicación configurada
    Console.WriteLine($"Aplicación configurada para escuchar en: {Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5000"}");

    // Agregar servicios al contenedor
    builder.Services.AgregarServiciosAplicacion(configuration);

    // Agregar los controladores
    builder.Services.AddControllers();

    // Construir la aplicación
    var app = builder.Build();

    // Configurar el middleware HTTP
    app.ConfigurarMiddleware();

    Console.WriteLine("Aplicación configurada correctamente. Iniciando...");

    // Ejecutar la aplicación
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error fatal al iniciar la aplicación: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}