using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El libro es obligatorio")]
        public int BookId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "La calificación es obligatoria")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(1000, ErrorMessage = "El comentario no puede exceder los 1000 caracteres")]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
