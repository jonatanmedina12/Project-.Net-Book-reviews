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
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar comportamiento de PostgreSQL para compatibilidad con Supabase
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            // Configurar PostgreSQL con Supabase (Transaction Pooler)
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(
                   configuration.GetConnectionString("DefaultConnection"),
                   npgsqlOptions =>
                   {
                       // Especificar el ensamblado para migraciones
                       npgsqlOptions.MigrationsAssembly("BookReviews.API");

                       // Aumentar timeout para operaciones de larga duración
                       npgsqlOptions.CommandTimeout(300);

                       // Especificar la tabla de migraciones y esquema
                       npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");

                       // Definir versión de PostgreSQL en Supabase
                       npgsqlOptions.SetPostgresVersion(14, 0);
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