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

namespace BookReviews.API.Extensions
{
    /// <summary>
    /// Clase estática que contiene extensiones para configurar servicios de la aplicación
    /// </summary>
    public static class ServiciosAplicacion
    {
        // Valores por defecto para las variables de entorno
        private const string DEFAULT_CONNECTION_VALUE = "User Id=postgres.bkitivgdezgwqqtwluky;Password=Jonatanmedina123__;Server=aws-0-us-east-2.pooler.supabase.com;Port=6543;Database=postgres;Timeout=300;Command Timeout=300;";
        private const string DIRECT_CONNECTION_VALUE = "Host=db.bkitivgdezgwqqtwluky.supabase.co;Database=postgres;Username=postgres;Password=Jonatanmedina123__;SSL Mode=Require;Trust Server Certificate=true";
        private const string JWT_SECRET_VALUE = "bdc5118737cbbd2b67026290dfe6de904d4fff8f2836c11425f6a2d1eeca42ced97d0afb748cf89c323df6b50f3e8432058f3a8992c6ce09c9ed8697ca75b7c9801fe5e536b2cc3499b8b2e43f5b5606ea27c9d0d071c69d4a013e151478535de1834d25f62add5528fe34421962cd63b13a802e07f4a15368fbf029c2ca3e4075c4351e161b85eaeb395a4719b34b4bad516dc47834dca43098dd6cf705f661cdb290d31f08c02f3ef34eb4d0986edfe504bfff9991ea748337717d30f84b3f82c9afda883da71bcd0d82f016a12f77ba51f01c91885f8253fd069b29add44b9d8d613555459a341421b2c62e96491c947968404bbcc07d030b65a05f25a1c0";
        private const string JWT_EXPIRY_MINUTES_VALUE = "60";
        private const string LOGGING_ENABLED_VALUE = "true";

        /// <summary>
        /// Carga las variables de entorno desde el archivo .env o utiliza valores por defecto
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IConfiguration CargarVariablesEntorno(this IConfiguration configuration)
        {
            try
            {
                // Verificar si estamos en Railway
                bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

                Console.WriteLine($"Detectado entorno Railway: {isRailway}");

                // Establecer valores en la configuración, priorizando variables de entorno existentes
                // sino usando los valores por defecto definidos como constantes

                // DEFAULT_CONNECTION
                string defaultConnection = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
                if (string.IsNullOrEmpty(defaultConnection))
                {
                    defaultConnection = DEFAULT_CONNECTION_VALUE;
                    Environment.SetEnvironmentVariable("DEFAULT_CONNECTION", defaultConnection);
                    Console.WriteLine("Usando valor por defecto para DEFAULT_CONNECTION");
                }
                configuration["ConnectionStrings:DefaultConnection"] = defaultConnection;
                Console.WriteLine("Variable DEFAULT_CONNECTION configurada correctamente.");

                // DIRECT_CONNECTION
                string directConnection = Environment.GetEnvironmentVariable("DIRECT_CONNECTION");
                if (string.IsNullOrEmpty(directConnection))
                {
                    directConnection = DIRECT_CONNECTION_VALUE;
                    Environment.SetEnvironmentVariable("DIRECT_CONNECTION", directConnection);
                    Console.WriteLine("Usando valor por defecto para DIRECT_CONNECTION");
                }
                configuration["ConnectionStrings:DirectConnection"] = directConnection;
                Console.WriteLine("Variable DIRECT_CONNECTION configurada correctamente.");

                // JWT_SECRET
                string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
                if (string.IsNullOrEmpty(jwtSecret))
                {
                    jwtSecret = JWT_SECRET_VALUE;
                    Environment.SetEnvironmentVariable("JWT_SECRET", jwtSecret);
                    Console.WriteLine("Usando valor por defecto para JWT_SECRET");
                }
                configuration["JWT:Secret"] = jwtSecret;
                Console.WriteLine("Variable JWT_SECRET configurada correctamente.");

                // JWT_EXPIRY_MINUTES
                string jwtExpiryMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");
                if (string.IsNullOrEmpty(jwtExpiryMinutes))
                {
                    jwtExpiryMinutes = JWT_EXPIRY_MINUTES_VALUE;
                    Environment.SetEnvironmentVariable("JWT_EXPIRY_MINUTES", jwtExpiryMinutes);
                    Console.WriteLine("Usando valor por defecto para JWT_EXPIRY_MINUTES");
                }
                configuration["JWT:ExpiryInMinutes"] = jwtExpiryMinutes;
                Console.WriteLine("Variable JWT_EXPIRY_MINUTES configurada correctamente.");

                // LOGGING_ENABLED
                string loggingEnabled = Environment.GetEnvironmentVariable("LOGGING_ENABLED");
                if (string.IsNullOrEmpty(loggingEnabled))
                {
                    loggingEnabled = LOGGING_ENABLED_VALUE;
                    Environment.SetEnvironmentVariable("LOGGING_ENABLED", loggingEnabled);
                    Console.WriteLine("Usando valor por defecto para LOGGING_ENABLED");
                }
                configuration["Logging:Enabled"] = loggingEnabled;
                Console.WriteLine("Variable LOGGING_ENABLED configurada correctamente.");

                // Mostrar valores configurados (con redacción para seguridad)
                Console.WriteLine("\nVariables de entorno configuradas:");
                Console.WriteLine($"DEFAULT_CONNECTION: {MascarValorSensible(defaultConnection)}");
                Console.WriteLine($"DIRECT_CONNECTION: {MascarValorSensible(directConnection)}");
                Console.WriteLine($"JWT_SECRET: {MascarValorSensible(jwtSecret)}");
                Console.WriteLine($"JWT_EXPIRY_MINUTES: {jwtExpiryMinutes}");
                Console.WriteLine($"LOGGING_ENABLED: {loggingEnabled}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar variables de entorno: {ex.Message}");
                Console.WriteLine("Usando valores por defecto para todas las variables");

                // En caso de error, asegurar que los valores por defecto se establezcan
                configuration["ConnectionStrings:DefaultConnection"] = DEFAULT_CONNECTION_VALUE;
                configuration["ConnectionStrings:DirectConnection"] = DIRECT_CONNECTION_VALUE;
                configuration["JWT:Secret"] = JWT_SECRET_VALUE;
                configuration["JWT:ExpiryInMinutes"] = JWT_EXPIRY_MINUTES_VALUE;
                configuration["Logging:Enabled"] = LOGGING_ENABLED_VALUE;
            }

            return configuration;
        }

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
            // Cargar variables de entorno
            configuration.CargarVariablesEntorno();

            // Registrar servicios de aplicación
            services.AddApplication();

            // Registrar servicios de infraestructura con el flag de migraciones
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
            // Obtener la clave JWT (ya debería estar configurada)
            string jwtSecret = configuration["JWT:Secret"];

            // Verificación adicional, aunque ya se debería haber manejado
            if (string.IsNullOrEmpty(jwtSecret))
            {
                Console.WriteLine("ADVERTENCIA: JWT:Secret no encontrado en configuración, usando valor por defecto.");
                jwtSecret = JWT_SECRET_VALUE;
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