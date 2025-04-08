using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviews.Domain.Entities
{
    /// <summary>
    /// Representa un libro en el sistema
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private Book() { }

        /// <summary>
        /// Constructor completo para crear un libro
        /// </summary>
        public Book(
            string title,
            string author,
            string summary,
            string isbn,
            int categoryId,
            ICollection<Review> reviews,
            string language,
            int publishedYear,
            string publisher,
            int pages)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Summary = summary;
            Isbn = isbn;
            CategoryId = categoryId;
            Reviews = reviews ?? new List<Review>();
            Language = language;
            PublishedYear = publishedYear;
            Publisher = publisher;
            Pages = pages;
        }

        /// <summary>
        /// Identificador único del libro
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        /// <summary>
        /// Título del libro
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; private set; }

        /// <summary>
        /// Autor del libro
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Author { get; private set; }

        /// <summary>
        /// Resumen del libro
        /// </summary>
        [MaxLength(1000)]
        public string Summary { get; private set; }

        /// <summary>
        /// Identificador de la categoría
        /// </summary>
        [Required]
        public int CategoryId { get; private set; }

        /// <summary>
        /// Categoría del libro
        /// </summary>
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; private set; }

        /// <summary>
        /// Colección de reseñas del libro
        /// </summary>
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        /// <summary>
        /// ISBN del libro
        /// </summary>
        [MaxLength(20)]
        public string Isbn { get; private set; }

        /// <summary>
        /// Idioma del libro
        /// </summary>
        [MaxLength(50)]
        public string Language { get; private set; }

        /// <summary>
        /// Año de publicación
        /// </summary>
        [Range(1000, 9999)]
        public int PublishedYear { get; private set; }

        /// <summary>
        /// Editorial
        /// </summary>
        [MaxLength(100)]
        public string Publisher { get; private set; }

        /// <summary>
        /// Número de páginas
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Pages { get; private set; }

        /// <summary>
        /// Ruta de la imagen de portada
        /// </summary>
        [MaxLength(500)]
        public string CoverImagePath { get; private set; }

        /// <summary>
        /// Método para actualizar la ruta de la imagen de portada
        /// </summary>
        /// <param name="coverImagePath">Nueva ruta de la imagen de portada</param>
        public void UpdateCoverImage(string coverImagePath)
        {
            CoverImagePath = coverImagePath;
        }

        /// <summary>
        /// Calcula la calificación promedio del libro
        /// </summary>
        /// <returns>Calificación promedio</returns>
        public double GetAverageRating()
        {
            if (Reviews == null || Reviews.Count == 0)
                return 0;

            return Reviews.Average(r => r.Rating);
        }

        /// <summary>
        /// Método para actualizar los detalles del libro
        /// </summary>
        public void UpdateDetails(
            string title,
            string author,
            string summary,
            int categoryId,
            string isbn,
            string language,
            int pages,
            int publishedYear,
            string publisher,
            string coverImagePath = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Summary = summary;
            CategoryId = categoryId;
            Isbn = isbn;
            Language = language;
            Pages = pages;
            PublishedYear = publishedYear;
            Publisher = publisher;

            if (!string.IsNullOrEmpty(coverImagePath))
            {
                CoverImagePath = coverImagePath;
            }
        }
    }
}