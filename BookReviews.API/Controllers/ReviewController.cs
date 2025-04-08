using BookReviews.Application.DTOs;
using BookReviews.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controlador para gestionar las reseñas de libros
/// </summary>
namespace BookReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Constructor del controlador de reseñas
        /// </summary>
        /// <param name="reviewService">Servicio de reseñas</param>
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Obtiene todas las reseñas de un libro específico
        /// </summary>
        /// <param name="bookId">ID del libro para obtener sus reseñas</param>
        /// <returns>Lista de reseñas del libro</returns>
        /// <response code="200">Reseñas obtenidas correctamente</response>
        /// <response code="404">Libro no encontrado</response>
        [HttpGet("book/{bookId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetBookReviews(int bookId)
        {
            var reviews = await _reviewService.GetBookReviewsAsync(bookId);
            return Ok(reviews);
        }

        /// <summary>
        /// Obtiene todas las reseñas del usuario actual
        /// </summary>
        /// <returns>Lista de reseñas del usuario</returns>
        /// <response code="200">Reseñas obtenidas correctamente</response>
        /// <response code="401">Usuario no autenticado</response>
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetUserReviews()
        {
            var userId = GetCurrentUserId();
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return Ok(reviews);
        }

        /// <summary>
        /// Crea una nueva reseña para un libro
        /// </summary>
        /// <param name="reviewDto">Datos de la reseña a crear</param>
        /// <returns>Información de la reseña creada</returns>
        /// <response code="201">Reseña creada correctamente</response>
        /// <response code="400">Datos de la reseña inválidos o ya existe una reseña del usuario para este libro</response>
        /// <response code="401">Usuario no autenticado</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = GetCurrentUserId();
                var createdReview = await _reviewService.CreateReviewAsync(reviewDto, userId);
                return CreatedAtAction(nameof(GetBookReviews), new { bookId = createdReview.BookId }, createdReview);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una reseña existente del usuario actual
        /// </summary>
        /// <param name="id">ID de la reseña a actualizar</param>
        /// <param name="reviewDto">Nuevos datos de la reseña</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Reseña actualizada correctamente</response>
        /// <response code="400">ID no coincide con la reseña o datos inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">La reseña no pertenece al usuario actual</response>
        /// <response code="404">Reseña no encontrada</response>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewDto reviewDto)
        {
            if (id != reviewDto.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = GetCurrentUserId();
                var result = await _reviewService.UpdateReviewAsync(reviewDto, userId);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Elimina una reseña del usuario actual
        /// </summary>
        /// <param name="id">ID de la reseña a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Reseña eliminada correctamente</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">La reseña no pertenece al usuario actual</response>
        /// <response code="404">Reseña no encontrada</response>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _reviewService.DeleteReviewAsync(id, userId);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
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