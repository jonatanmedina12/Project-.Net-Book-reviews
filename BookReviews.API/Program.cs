using BookReviews.API.Extensions;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

try
{
    Console.WriteLine("Iniciando programa principal");

    // Crear el builder de la aplicación
    Console.WriteLine("Creando WebApplicationBuilder");
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog para logging
    Console.WriteLine("Configurando Serilog");
    builder.ConfigurarSerilog();
    Log.Information("Iniciando aplicación Book Reviews API");

    // Configurar y agregar servicios de la aplicación
    Console.WriteLine("Agregando servicios de la aplicación");
    try
    {
        builder.Services.AgregarServiciosAplicacion(builder.Configuration);
        Console.WriteLine("Servicios de aplicación agregados correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR al agregar servicios: {ex.Message}");
        Log.Error(ex, "Error al agregar servicios");
        throw; // Re-lanzar para el manejo general
    }

    // Agregar controladores para la API
    Console.WriteLine("Agregando controladores");
    builder.Services.AddControllers();

    // Construir la aplicación
    Console.WriteLine("Construyendo la aplicación");
    var app = builder.Build();
    Console.WriteLine("Aplicación construida correctamente");

    // Configurar el middleware
    Console.WriteLine("Configurando middleware");
    try
    {
        app.ConfigurarMiddleware();
        Console.WriteLine("Middleware configurado correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR al configurar middleware: {ex.Message}");
        Log.Error(ex, "Error al configurar middleware");
        throw; // Re-lanzar para el manejo general
    }

    // Iniciar la aplicación
    Console.WriteLine("Iniciando la aplicación web");
    app.Run();

    return 0;
}
catch (Exception ex)
{
    // Mostrar detalles más específicos del error
    Console.WriteLine($"ERROR FATAL: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");

    Log.Fatal(ex, "Error detallado: {ErrorMessage}", ex.ToString());

    // Si hay una excepción interna, mostrarla también
    if (ex.InnerException != null)
    {
        Console.WriteLine($"ERROR INTERNO: {ex.InnerException.Message}");
        Console.WriteLine($"Stack trace interno: {ex.InnerException.StackTrace}");

        Log.Fatal(ex.InnerException, "Error interno: {InnerErrorMessage}", ex.InnerException.ToString());
    }

    return 1;
}
finally
{
    Console.WriteLine("Finalizando programa");
    Log.CloseAndFlush();
}