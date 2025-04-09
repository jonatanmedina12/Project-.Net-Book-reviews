using BookReviews.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<string>> TestConnection()
    {
        try
        {
            // Intenta acceder a la base de datos
            bool canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
                return Ok("Conexión a la base de datos establecida correctamente");
            else
                return BadRequest("No se pudo conectar a la base de datos");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}