using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviews.Domain.Entities
{
    /// <summary>
    /// Representa una reseña de un libro
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Identificador único de la reseña
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        /// <summary>
        /// Calificación de la reseña
        /// </summary>
        [Required]
        public Rating Rating { get; private set; }

        /// <summary>
        /// Comentario de la reseña
        /// </summary>
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "El comentario debe tener entre 1 y 1000 caracteres")]
        public string Comment { get; private set; }

        /// <summary>
        /// Fecha de creación de la reseña
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Fecha de última actualización de la reseña
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Identificador del libro reseñado
        /// </summary>
        [Required]
        public int BookId { get; private set; }

        /// <summary>
        /// Libro reseñado
        /// </summary>
        [ForeignKey(nameof(BookId))]
        public Book Book { get; private set; }

        /// <summary>
        /// Identificador del usuario que realiza la reseña
        /// </summary>
        [Required]
        public int UserId { get; private set; }

        /// <summary>
        /// Usuario que realiza la reseña
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }

        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private Review() { }

        /// <summary>
        /// Constructor para crear una nueva reseña
        /// </summary>
        /// <param name="bookId">Identificador del libro</param>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="ratingValue">Valor de la calificación</param>
        /// <param name="comment">Comentario de la reseña</param>
        /// <exception cref="ArgumentException">Se lanza si el comentario no es válido</exception>
        public Review(int bookId, int userId, int ratingValue, string comment)
        {
            ValidateComment(comment);

            BookId = bookId;
            UserId = userId;
            Rating = new Rating(ratingValue);
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Actualiza los detalles de una reseña existente
        /// </summary>
        /// <param name="ratingValue">Nuevo valor de calificación</param>
        /// <param name="comment">Nuevo comentario</param>
        /// <exception cref="ArgumentException">Se lanza si el comentario no es válido</exception>
        public void Update(int ratingValue, string comment)
        {
            ValidateComment(comment);

            Rating = new Rating(ratingValue);
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Valida el comentario de la reseña
        /// </summary>
        /// <param name="comment">Comentario a validar</param>
        /// <exception cref="ArgumentException">Se lanza si el comentario no es válido</exception>
        private void ValidateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment) || comment.Length > 1000)
                throw new ArgumentException("El comentario debe tener entre 1 y 1000 caracteres", nameof(comment));
        }
    }
}