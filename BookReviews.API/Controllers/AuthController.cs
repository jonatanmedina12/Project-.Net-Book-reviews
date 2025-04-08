using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para gestionar la autenticación y registro de usuarios
/// </summary>

namespace BookReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        /// <summary>
        /// Constructor del controlador de autenticación
        /// </summary>
        /// <param name="authService">Servicio de autenticación</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="registerDto">Datos de registro del usuario</param>
        /// <returns>Información del usuario registrado</returns>
        /// <response code="201">Usuario registrado correctamente</response>
        /// <response code="400">Datos de registro inválidos</response>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(Register), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Autentica a un usuario y genera un token JWT
        /// </summary>
        /// <param name="loginDto">Credenciales de inicio de sesión</param>
        /// <returns>Token de autenticación</returns>
        /// <response code="200">Inicio de sesión exitoso</response>
        /// <response code="401">Credenciales inválidas</response>
        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Credenciales inválidas");
            }
        }
        /// <summary>
        /// Solicita un restablecimiento de contraseña
        /// </summary>
        /// <param name="forgotPasswordDto">Email del usuario</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">Solicitud procesada correctamente</response>
        /// <response code="400">Email no encontrado o inválido</response>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
                return Ok(new { message = "Se ha enviado un enlace de restablecimiento a tu correo electrónico" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Restablece la contraseña de un usuario
        /// </summary>
        /// <param name="resetPasswordDto">Datos para restablecer la contraseña</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">Contraseña restablecida con éxito</response>
        /// <response code="400">Datos inválidos o token expirado</response>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.ResetPasswordAsync(resetPasswordDto);
                return Ok(new { message = "Contraseña restablecida con éxito" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
