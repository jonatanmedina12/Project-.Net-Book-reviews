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
            CreateMap<Book, BookDto>()
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImagePath))
            .ForMember(dest => dest.CoverImage, opt => opt.Ignore());
            // Mapeo de BookDto a Book
            CreateMap<BookDto, Book>()
                .ConstructUsing(src => new Book(
                    src.Title,
                    src.Author,
                    src.Summary,
                    src.Isbn,
                    src.CategoryId,
                    null, // Reviews se establecen separadamente
                    src.Language,
                    src.PublishedYear,
                    src.Publisher,
                    src.Pages))
                .ForMember(dest => dest.CoverImagePath, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());

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
