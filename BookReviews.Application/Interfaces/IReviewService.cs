using BookReviews.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetBookReviewsAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId);
        Task<ReviewDto> CreateReviewAsync(ReviewDto reviewDto, int userId);
        Task<bool> UpdateReviewAsync(ReviewDto reviewDto, int userId);
        Task<bool> DeleteReviewAsync(int id, int userId);
    }
}
