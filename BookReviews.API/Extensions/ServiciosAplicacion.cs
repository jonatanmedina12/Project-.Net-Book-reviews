using BookReviews.API.Utilities;
using BookReviews.Application;
using BookReviews.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using DotNetEnv;

namespace BookReviews.API.Extensions
{
    /// <summary>
    /// Clase estática que contiene extensiones para configurar servicios de la aplicación
    /// </summary>
    public static class ServiciosAplicacion
    {
        /// <summary>
        /// Carga las variables de entorno desde el archivo .env y las integra con la configuración
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La configuración actualizada</returns>
        public static IConfiguration CargarVariablesEntorno(this IConfiguration configuration)
        {
            try
            {
                // Cargar archivo .env si existe (primero buscar en la raíz del proyecto)
                string envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                bool envFileExists = File.Exists(envPath);

                if (envFileExists)
                {
                    // Cargar variables desde .env en las variables de entorno
                    Env.Load(envPath);
                    Console.WriteLine($"Archivo .env cargado desde: {envPath}");

                    // Mapear variables específicas del .env al formato esperado por .NET
                    MapearVariablesEntorno();

                    // Verificar si estamos en Railway u otro entorno de despliegue
                    bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

                    if (isRailway)
                    {
                        Console.WriteLine("Ejecutando en entorno Railway");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró archivo .env. Se usarán valores de appsettings.json o variables de entorno.");
                }

                // Verificar y registrar las variables de entorno clave
                VerificarVariablesEntorno();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar variables de entorno: {ex.Message}");
                Console.WriteLine("Se continuará con los valores existentes en la configuración.");
            }

            return configuration;
        }

        /// <summary>
        /// Convierte un diccionario jerárquico en un diccionario plano con claves separadas por ":"
        /// </summary>
        private static Dictionary<string, string> FlattenDictionary(Dictionary<string, object> dict, string prefix = "")
        {
            var result = new Dictionary<string, string>();

            foreach (var entry in dict)
            {
                string key = string.IsNullOrEmpty(prefix) ? entry.Key : $"{prefix}:{entry.Key}";

                if (entry.Value is Dictionary<string, object> nestedDict)
                {
                    var nestedResult = FlattenDictionary(nestedDict, key);
                    foreach (var nestedEntry in nestedResult)
                    {
                        result[nestedEntry.Key] = nestedEntry.Value;
                    }
                }
                else
                {
                    result[key] = entry.Value?.ToString();
                }
            }

            return result;
        }


        /// <summary>
        /// Mapea las variables de .env al formato esperado por .NET
        /// </summary>
        private static void MapearVariablesEntorno()
        {
            // Mapeo para ConnectionStrings
            string defaultConnection = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
            if (!string.IsNullOrEmpty(defaultConnection))
            {
                Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", defaultConnection);
                Console.WriteLine("Variable DEFAULT_CONNECTION mapeada a ConnectionStrings__DefaultConnection");
            }

            // Mapeo para JWT
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (!string.IsNullOrEmpty(jwtSecret))
            {
                Environment.SetEnvironmentVariable("JWT__Secret", jwtSecret);
                Console.WriteLine("Variable JWT_SECRET mapeada a JWT__Secret");
            }

            string jwtExpiryMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");
            if (!string.IsNullOrEmpty(jwtExpiryMinutes))
            {
                Environment.SetEnvironmentVariable("JWT__ExpiryMinutes", jwtExpiryMinutes);
                Console.WriteLine("Variable JWT_EXPIRY_MINUTES mapeada a JWT__ExpiryMinutes");
            }

            // Mapeo para Logging
            string loggingEnabled = Environment.GetEnvironmentVariable("LOGGING_ENABLED");
            if (!string.IsNullOrEmpty(loggingEnabled))
            {
                Environment.SetEnvironmentVariable("Logging__Enabled", loggingEnabled);
                Console.WriteLine("Variable LOGGING_ENABLED mapeada a Logging__Enabled");
            }
        }

        /// <summary>
        /// Verifica la presencia de variables de entorno clave y registra su estado
        /// </summary>
        private static void VerificarVariablesEntorno()
        {
            // Lista de variables clave que deberían estar configuradas
            string[] variablesClave = new[]
            {
                "ConnectionStrings__DefaultConnection",
                "JWT__Secret",
                "JWT__ExpiryMinutes",
                "Logging__Enabled"
            };

            foreach (var variable in variablesClave)
            {
                string valor = Environment.GetEnvironmentVariable(variable);

                // Registrar si la variable está presente (sin mostrar su valor completo por seguridad)
                if (valor != null)
                {
                    string valorEnmascarado = MascarValorSensible(valor);
                    Console.WriteLine($"Variable de entorno {variable}: Configurada ({valorEnmascarado})");
                }
                else
                {
                    Console.WriteLine($"Variable de entorno {variable}: NO configurada");
                }
            }
        }
        /// <summary>


        /// <summary>
        /// Configura todos los servicios necesarios para la aplicación
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La colección de servicios con todos los servicios registrados</returns>
        public static IServiceCollection AgregarServiciosAplicacion(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Cargar variables de entorno e integrarlas con la configuración
            configuration.CargarVariablesEntorno();

            // Registrar servicios de aplicación
            services.AddApplication();

            // Registrar servicios de infraestructura
            services.AddInfrastructure(configuration);

            // Configurar autenticación JWT
            ConfigurarAutenticacionJWT(services, configuration);

            // Configurar Swagger
            ConfigurarSwagger(services);

            // Configurar CORS
            ConfigurarCORS(services);

            return services;
        }

        /// <summary>
        /// Configura Serilog para el registro de logs
        /// </summary>
        /// <param name="builder">WebApplicationBuilder</param>
        /// <returns>El WebApplicationBuilder configurado</returns>
        public static WebApplicationBuilder ConfigurarSerilog(this WebApplicationBuilder builder)
        {
            // Cargar variables de entorno antes de configurar Serilog
            builder.Configuration.CargarVariablesEntorno();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/bookreview-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }

        /// <summary>
        /// Configura el middleware de la aplicación
        /// </summary>
        /// <param name="app">WebApplication</param>
        /// <returns>El WebApplication configurado</returns>
        public static WebApplication ConfigurarMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Review API v1"));

            app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();
            app.UseMiddleware<Middleware.RequestLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication(); // Antes de UseAuthorization

            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));

            return app;
        }

        #region Métodos privados de configuración

        /// <summary>
        /// Configura la autenticación JWT
        /// </summary>
        private static void ConfigurarAutenticacionJWT(IServiceCollection services, IConfiguration configuration)
        {
            // Obtener la clave JWT
            string jwtSecret = configuration["JWT:Secret"];

            // Verificación de seguridad
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("La clave JWT:Secret no está configurada. Verifique su archivo appsettings.json o variables de entorno.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role
                    };

                    // Eventos para debugging
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Autenticación fallida: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("Token validado correctamente");
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        /// <summary>
        /// Configura Swagger para la documentación de la API
        /// </summary>
        private static void ConfigurarSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Book Reviews API",
                    Version = "v1",
                    Description = "API para un sistema de reseñas de libros",
                    Contact = new OpenApiContact
                    {
                        Name = "Jonatan Albenio Medina",
                        Email = "jonatanalbeniomedina@outlook.com"
                    }
                });

                // Incluir archivo XML de documentación
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Verificar si el archivo existe antes de incluirlo
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                else
                {
                    // Registrar una advertencia de que falta el archivo XML
                    Console.WriteLine($"ADVERTENCIA: Archivo de documentación XML no encontrado en {xmlPath}");
                }

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });
            });
        }

        /// <summary>
        /// Configura la política CORS
        /// </summary>
        private static void ConfigurarCORS(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        /// <summary>
        /// Enmascara un valor sensible para mostrarlo en logs
        /// </summary>
        /// <param name="valor">Valor a enmascarar</param>
        /// <returns>Valor enmascarado</returns>
        private static string MascarValorSensible(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return "[vacío]";

            if (valor.Length <= 10)
                return "********";

            // Mostrar los primeros 4 caracteres y enmascarar el resto
            return valor.Substring(0, 4) + "****************";
        }

        #endregion
    }
}