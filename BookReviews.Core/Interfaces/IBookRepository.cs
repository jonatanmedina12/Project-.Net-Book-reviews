using BookReviews.Domain.Entities;


namespace BookReviews.Core.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetBookWithReviewsAsync(int id);
        Task<IReadOnlyList<Book>> GetAllBooksAsync();
        Task<IReadOnlyList<Book>> SearchBooksAsync(string searchTerm, int? categoryId);
        Task<Book> AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Book book);
    }
}
