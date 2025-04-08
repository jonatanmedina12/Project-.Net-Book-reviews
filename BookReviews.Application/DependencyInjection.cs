using AutoMapper;
using BookReviews.Application.Interfaces;
using BookReviews.Application.Mappings;
using BookReviews.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;


namespace BookReviews.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar servicios
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWebRootProvider, WebRootProvider>();


            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            }).CreateMapper());

            // Registrar validadores
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
