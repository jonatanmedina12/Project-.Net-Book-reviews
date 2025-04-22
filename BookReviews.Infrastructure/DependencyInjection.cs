using BookReviews.API.Extensions;
using BookReviews.Application.Interfaces;
using BookReviews.Core.Interfaces;
using BookReviews.Infrastructure.Data;
using BookReviews.Infrastructure.Data.Repositories;
using BookReviews.Infrastructure.Identity;
using BookReviews.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookReviews.Infrastructure
{
    /// <summary>
    /// Clase estática para registrar los servicios de infraestructura
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registra los servicios de infraestructura en el contenedor de DI
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>La colección de servicios con los servicios de infraestructura registrados</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar comportamiento de PostgreSQL (ajustes generales)
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseNpgsql(
                     configuration.GetConnectionString("DefaultConnection"),
                     npgsqlOptions =>
                     {
                         // Cambiar a Infrastructure en lugar de API
                         npgsqlOptions.MigrationsAssembly("BookReviews.Infrastructure");

                         // Timeout para operaciones (reducido para entorno local)
                         npgsqlOptions.CommandTimeout(60);

                         // Especificar la tabla de migraciones y esquema
                         npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                     }));

            // Registrar repositorios
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            // Registrar servicios de infraestructura
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ILoggerService, LoggingService>();

            return services;
        }
    }
}