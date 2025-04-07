using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Domain.Entities
{
    public class Review
    {
        public int Id { get; private set; }
        public Rating Rating { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public int BookId { get; private set; }
        public Book Book { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }

        private Review() { }

        public Review(int bookId, int userId, int ratingValue, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment) || comment.Length > 1000)
                throw new ArgumentException("El comentario debe tener entre 1 y 1000 caracteres", nameof(comment));

            BookId = bookId;
            UserId = userId;
            Rating = new Rating(ratingValue);
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(int ratingValue, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment) || comment.Length > 1000)
                throw new ArgumentException("El comentario debe tener entre 1 y 1000 caracteres", nameof(comment));

            Rating = new Rating(ratingValue);
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
