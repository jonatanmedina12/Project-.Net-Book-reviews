using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviews.Domain.Entities
{
    /// <summary>
    /// Representa una calificación para una reseña
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Identificador único de la calificación
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        /// <summary>
        /// Valor de la calificación
        /// </summary>
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Value { get; private set; }

        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private Rating() { }

        /// <summary>
        /// Constructor para crear una nueva calificación
        /// </summary>
        /// <param name="value">Valor de la calificación</param>
        /// <exception cref="ArgumentException">Se lanza si el valor está fuera del rango válido</exception>
        public Rating(int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Convierte implícitamente el valor de calificación a long
        /// </summary>
        /// <param name="rating">Calificación a convertir</param>
        public static implicit operator long(Rating rating)
        {
            return rating?.Value ?? 0;
        }

        /// <summary>
        /// Convierte implícitamente el valor de calificación a double
        /// </summary>
        /// <param name="rating">Calificación a convertir</param>
        public static implicit operator double(Rating rating)
        {
            return rating?.Value ?? 0;
        }
    }
}