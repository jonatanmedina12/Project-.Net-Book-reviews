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
        /// <summary>
        /// Carga las variables de entorno desde el archivo .env
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IConfiguration CargarVariablesEntorno(this IConfiguration configuration)
        {
            // No necesitamos cargar DotEnv.Load() aquí, ya lo hicimos en Program.cs

            // Verificar si estamos en Railway
            bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                             !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

            // Si estamos en Railway, las variables ya deberían estar disponibles
            if (isRailway)
            {
                Console.WriteLine("Detectado entorno Railway. Usando variables de entorno proporcionadas por Railway.");
            }

            // Reemplazar variables en la configuración (esto funciona tanto para .env como para Railway)
            if (Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") != null)
            {
                configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
                Console.WriteLine("Variable DEFAULT_CONNECTION cargada correctamente.");
            }

            if (Environment.GetEnvironmentVariable("DIRECT_CONNECTION") != null)
            {
                configuration["ConnectionStrings:DirectConnection"] = Environment.GetEnvironmentVariable("DIRECT_CONNECTION");
                Console.WriteLine("Variable DIRECT_CONNECTION cargada correctamente.");
            }

            if (Environment.GetEnvironmentVariable("JWT_SECRET") != null)
            {
                configuration["JWT:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");
                Console.WriteLine("Variable JWT_SECRET cargada correctamente.");
            }

            if (Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") != null)
            {
                configuration["JWT:ExpiryInMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");
                Console.WriteLine("Variable JWT_EXPIRY_MINUTES cargada correctamente.");
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
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Review API v1"));
            }

            app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();
            app.UseMiddleware<Middleware.RequestLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication(); // Antes de UseAuthorization

            app.UseAuthorization();
            app.MapControllers();

            return app;
        }

        #region Métodos privados de configuración

        /// <summary>
        /// Configura la autenticación JWT
        /// </summary>
        private static void ConfigurarAutenticacionJWT(IServiceCollection services, IConfiguration configuration)
        {
            // Verificar si la clave JWT está configurada
            string jwtSecret = configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                Console.WriteLine("ERROR: JWT:Secret no está configurado o está vacío");
                // Usar una clave predeterminada para desarrollo (NO HACER ESTO EN PRODUCCIÓN)
                jwtSecret = "ClaveDeDesarrolloTemporalNoUsarEnProduccion123456789012345678901234";
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
                            foreach (var claim in context.Principal.Claims)
                            {
                                Console.WriteLine($"{claim.Type}: {claim.Value}");
                            }
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

        #endregion
    }
}