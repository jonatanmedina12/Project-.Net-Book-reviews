using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.DTOs
{
    /// <summary>
    /// Datos para el registro de un nuevo usuario en el sistema
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Nombre de usuario. Debe ser único en el sistema.
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico del usuario. Debe ser único en el sistema.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña del usuario. Debe cumplir con los requisitos de seguridad.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de contraseña. Debe coincidir con la contraseña proporcionada.
        /// </summary>
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Rol del usuario en el sistema
        /// </summary>
        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; }
    }
}
