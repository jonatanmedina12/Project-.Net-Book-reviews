using BookReviews.API.Extensions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Serilog;

try
{
    // Crear el builder de la aplicación
    var builder = WebApplication.CreateBuilder(args);

    // Configurar las variables de entorno antes de configurar otros servicios
    // Usamos el método para procesar las variables, pero no intentamos reemplazar Configuration
    builder.Configuration.CargarVariablesEntorno();

    // Configurar Serilog para logging
    builder.ConfigurarSerilog();
    Log.Information("Iniciando aplicación Book Reviews API");

    // Configurar y agregar servicios de la aplicación
    builder.Services.AgregarServiciosAplicacion(builder.Configuration);
    builder.Services.AddControllers();

    var app = builder.Build();
    app.ConfigurarMiddleware();

    // Registrar un callback para cuando la aplicación esté lista y mostrar las URLs
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
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
                        Console.WriteLine($"Swagger UI disponible en: {address.TrimEnd('/')}/swagger");
                    }
                    Console.WriteLine("======================================");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning("No se pudo obtener información de direcciones del servidor: {Error}", ex.Message);
        }
    });

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error detallado: {ErrorMessage}", ex.ToString());

    if (ex.InnerException != null)
    {
        Log.Fatal(ex.InnerException, "Error interno: {InnerErrorMessage}", ex.InnerException.ToString());
    }
    return 1;
}
finally
{
    Log.CloseAndFlush();
}