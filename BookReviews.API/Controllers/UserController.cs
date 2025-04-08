using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controlador para gestionar las operaciones relacionadas con el perfil de usuario
/// </summary>
namespace BookReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        /// <param name="userService">Servicio de usuarios</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        /// <returns>Información del perfil del usuario</returns>
        /// <response code="200">Perfil obtenido correctamente</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Actualiza la información del perfil del usuario actual
        /// </summary>
        /// <param name="updateProfileDto">Datos actualizados del perfil</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Perfil actualizado correctamente</response>
        /// <response code="400">Datos del perfil inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userService.UpdateProfileAsync(userId, updateProfileDto);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        /// <param name="changePasswordDto">Datos de cambio de contraseña</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Contraseña cambiada correctamente</response>
        /// <response code="400">Datos inválidos o contraseña actual incorrecta</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="404">Usuario no encontrado</response>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("La contraseña actual es incorrecta");
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual a partir de los claims
        /// </summary>
        /// <returns>ID del usuario autenticado</returns>
        /// <exception cref="InvalidOperationException">Si no se puede obtener el ID del usuario</exception>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new InvalidOperationException("No se pudo obtener el ID del usuario");
            }
            return userId;
        }
    }
}