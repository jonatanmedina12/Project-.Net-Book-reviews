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
            // Configurar PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Registrar repositorios
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registrar servicios de infraestructura
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ILoggerService, LoggingService>();

            return services;
        }
    }
}
