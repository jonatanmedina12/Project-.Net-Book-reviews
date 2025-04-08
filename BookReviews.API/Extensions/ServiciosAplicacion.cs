using BookReviews.Application;
using BookReviews.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
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
                        ClockSkew = TimeSpan.Zero
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book Review API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
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
                        Array.Empty<string>()
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
