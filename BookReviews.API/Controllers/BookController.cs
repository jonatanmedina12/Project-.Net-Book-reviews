using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para gestionar los libros en el sistema
/// </summary>
namespace BookReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        /// <summary>
        /// Constructor del controlador de libros
        /// </summary>
        /// <param name="bookService">Servicio de libros</param>
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Obtiene todos los libros con filtros opcionales
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda para filtrar libros</param>
        /// <param name="categoryId">ID de categoría para filtrar libros</param>
        /// <returns>Lista de libros que coinciden con los criterios</returns>
        /// <response code="200">Libros obtenidos correctamente</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int? categoryId)
        {
            var books = await _bookService.GetBooksAsync(searchTerm, categoryId);
            return Ok(books);
        }

        /// <summary>
        /// Obtiene un libro por su ID
        /// </summary>
        /// <param name="id">ID del libro a obtener</param>
        /// <returns>Información detallada del libro</returns>
        /// <response code="200">Libro encontrado correctamente</response>
        /// <response code="404">Libro no encontrado</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        /// <summary>
        /// Crea un nuevo libro en el sistema
        /// </summary>
        /// <param name="bookDto">Datos del libro a crear</param>
        /// <returns>Información del libro creado</returns>
        /// <response code="201">Libro creado correctamente</response>
        /// <response code="400">Datos del libro inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdBook = await _bookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        /// <summary>
        /// Actualiza la información de un libro existente
        /// </summary>
        /// <param name="id">ID del libro a actualizar</param>
        /// <param name="bookDto">Nuevos datos del libro</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Libro actualizado correctamente</response>
        /// <response code="400">ID no coincide con el libro o datos inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        /// <response code="404">Libro no encontrado</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            if (id != bookDto.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _bookService.UpdateBookAsync(bookDto);
            if (!result)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Elimina un libro del sistema
        /// </summary>
        /// <param name="id">ID del libro a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Libro eliminado correctamente</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        /// <response code="404">Libro no encontrado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}