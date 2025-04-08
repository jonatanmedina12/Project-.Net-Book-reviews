using AutoMapper;
using BookReviews.Application.DTOs;
using BookReviews.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.GetAverageRating()))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));

            // Review
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.Value))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            // User
            CreateMap<User, UserDto>();

            // Category
            CreateMap<Category, CategoryDto>();
        }
    }
}
