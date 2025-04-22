using BookReviews.API.Extensions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Serilog;
using System;
using System.Linq;

try
{
    Console.WriteLine("Iniciando programa principal");

    // Crear el builder de la aplicaci�n
    Console.WriteLine("Creando WebApplicationBuilder");
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog para logging
    Console.WriteLine("Configurando Serilog");
    builder.ConfigurarSerilog();
    Log.Information("Iniciando aplicaci�n Book Reviews API");

    // Configurar y agregar servicios de la aplicaci�n
    Console.WriteLine("Agregando servicios de la aplicaci�n");
    try
    {
        builder.Services.AgregarServiciosAplicacion(builder.Configuration);
        Console.WriteLine("Servicios de aplicaci�n agregados correctamente");
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

    // Construir la aplicaci�n
    Console.WriteLine("Construyendo la aplicaci�n");
    var app = builder.Build();
    Console.WriteLine("Aplicaci�n construida correctamente");

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

    // Iniciar la aplicaci�n
    Console.WriteLine("Iniciando la aplicaci�n web");

    // Registrar un callback para cuando la aplicaci�n est� lista y mostrar las URLs
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var server = app.Services.GetService(typeof(IServer)) as IServer;
        if (server != null)
        {
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            if (addressFeature != null)
            {
                Console.WriteLine("===== SERVIDOR WEB INICIALIZADO =====");
                foreach (var address in addressFeature.Addresses)
                {
                    Console.WriteLine($"Escuchando en: {address}");

                    // Crear URL para Swagger
                    string swaggerUrl = address.TrimEnd('/') + "/swagger";
                    Console.WriteLine($"Swagger UI disponible en: {swaggerUrl}");
                }
                Console.WriteLine("======================================");
            }
            else
            {
                Console.WriteLine("No se pudo obtener informaci�n de direcciones del servidor");
            }
        }
        else
        {
            Console.WriteLine("No se pudo obtener el servicio de servidor");
        }
    });

    app.Run();

    return 0;
}
catch (Exception ex)
{
    // Mostrar detalles m�s espec�ficos del error
    Console.WriteLine($"ERROR FATAL: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");

    Log.Fatal(ex, "Error detallado: {ErrorMessage}", ex.ToString());

    // Si hay una excepci�n interna, mostrarla tambi�n
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