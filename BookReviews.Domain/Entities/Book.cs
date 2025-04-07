using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Domain.Entities
{
    public class Book
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Summary { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        private Book() { }

        public Book(string title, string author, string summary, int categoryId)
        {
            ValidateTitle(title);
            ValidateAuthor(author);

            Title = title;
            Author = author;
            Summary = summary;
            CategoryId = categoryId;
        }

        public void UpdateDetails(string title, string author, string summary, int categoryId)
        {
            ValidateTitle(title);
            ValidateAuthor(author);

            Title = title;
            Author = author;
            Summary = summary;
            CategoryId = categoryId;
        }

        public double GetAverageRating()
        {
            return Reviews.Any() ? Reviews.Average(r => r.Rating.Value) : 0;
        }

        private void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título del libro no puede estar vacío", nameof(title));
            if (title.Length > 200)
                throw new ArgumentException("El título no puede exceder los 200 caracteres", nameof(title));
        }

        private void ValidateAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("El autor del libro no puede estar vacío", nameof(author));
            if (author.Length > 150)
                throw new ArgumentException("El nombre del autor no puede exceder los 150 caracteres", nameof(author));
        }
    }
}
