using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150, ErrorMessage = "El autor no puede exceder los 150 caracteres")]
        public string Author { get; set; }

        [StringLength(2000, ErrorMessage = "El resumen no puede exceder los 2000 caracteres")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public List<ReviewDto> Reviews { get; set; }
    }
}
