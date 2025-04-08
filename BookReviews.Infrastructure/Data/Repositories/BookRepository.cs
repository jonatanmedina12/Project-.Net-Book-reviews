using BookReviews.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BookReviews.Core.Interfaces;
namespace BookReviews.Infrastructure.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> GetBookWithReviewsAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IReadOnlyList<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Book>> SearchBooksAsync(string searchTerm, int? categoryId)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            return await query
              .Include(b => b.Category)
              .Include(b => b.Reviews)  // Incluir las reseñas
                  .ThenInclude(r => r.User)  // Opcionalmente incluir los usuarios de las reseñas
              .ToListAsync();
        }

        public async Task<Book> AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            return book;
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
        }

        public async Task DeleteAsync(Book book)
        {
            _context.Books.Remove(book);
        }
    }
}
