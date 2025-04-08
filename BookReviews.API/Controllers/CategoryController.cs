using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor del controlador de categorías
        /// </summary>
        /// <param name="categoryService">Servicio de categorías</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        /// <returns>Lista de categorías</returns>
        /// <response code="200">Categorías obtenidas correctamente</response>
        [HttpGet]
        [AllowAnonymous] // No requiere autenticación
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        /// <param name="id">ID de la categoría a obtener</param>
        /// <returns>Información de la categoría</returns>
        /// <response code="200">Categoría encontrada correctamente</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpGet("{id}")]
        [AllowAnonymous] // No requiere autenticación
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        /// <param name="categoryDto">Datos de la categoría a crear</param>
        /// <returns>Información de la categoría creada</returns>
        /// <response code="201">Categoría creada correctamente</response>
        /// <response code="400">Datos de la categoría inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría a actualizar</param>
        /// <param name="categoryDto">Nuevos datos de la categoría</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Categoría actualizada correctamente</response>
        /// <response code="400">ID no coincide con la categoría o datos inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryDto);
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
        /// Elimina una categoría
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Categoría eliminada correctamente</response>
        /// <response code="400">No se puede eliminar la categoría debido a restricciones</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="403">Usuario no autorizado</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
