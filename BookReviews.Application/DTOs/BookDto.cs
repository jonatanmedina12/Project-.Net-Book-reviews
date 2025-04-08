using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookReviews.Application.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para libros
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// Identificador único del libro
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título del libro
        /// </summary>
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        public string Title { get; set; }

        /// <summary>
        /// Autor del libro
        /// </summary>
        [Required(ErrorMessage = "El autor es requerido")]
        [StringLength(150, ErrorMessage = "El nombre del autor no puede exceder los 150 caracteres")]
        public string Author { get; set; }

        /// <summary>
        /// Resumen del libro
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Identificador de la categoría del libro
        /// </summary>
        [Required(ErrorMessage = "La categoría es requerida")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Nombre de la categoría del libro
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Número de identificación Internacional para libros (ISBN)
        /// </summary>
        [StringLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres")]
        public string Isbn { get; set; }

        /// <summary>
        /// Idioma del libro
        /// </summary>
        [StringLength(50, ErrorMessage = "El idioma no puede exceder los 50 caracteres")]
        public string Language { get; set; }

        /// <summary>
        /// Número de páginas del libro
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El número de páginas debe ser un número positivo")]
        public int Pages { get; set; }

        /// <summary>
        /// Año de publicación del libro
        /// </summary>
        [Range(1000, 9999, ErrorMessage = "El año de publicación debe ser un año válido")]
        public int PublishedYear { get; set; }

        /// <summary>
        /// Editorial del libro
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre de la editorial no puede exceder los 100 caracteres")]
        public string Publisher { get; set; }

        /// <summary>
        /// Archivo de imagen de portada para subir
        /// </summary>
        public IFormFile CoverImage { get; set; }

        /// <summary>
        /// URL de la imagen de portada
        /// </summary>
        public string CoverImageUrl { get; set; }

        /// <summary>
        /// Calificación promedio del libro
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Número de reseñas del libro
        /// </summary>
        public int ReviewCount { get; set; }
    }
}