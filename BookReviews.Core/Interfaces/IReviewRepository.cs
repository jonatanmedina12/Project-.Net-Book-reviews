using BookReviews.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(int id);
        Task<IReadOnlyList<Review>> GetReviewsByBookAsync(int bookId);
        Task<IReadOnlyList<Review>> GetReviewsByUserAsync(int userId);
        Task<bool> HasUserReviewedBookAsync(int userId, int bookId);
        Task<Review> AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}
