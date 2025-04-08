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
        /// Configura todos los servicios necesarios para la aplicación
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La colección de servicios con todos los servicios registrados</returns>
        public static IServiceCollection AgregarServiciosAplicacion(this IServiceCollection services, IConfiguration configuration)
        {
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role // Asegúrate de que esté configurado correctamente
                    };

                    // Añade eventos para debugging
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
                            // Puedes imprimir los claims para verificar si está presente el rol
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
