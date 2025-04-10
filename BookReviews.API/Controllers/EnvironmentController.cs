using BookReviews.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookReviews.API.Controllers
{
    /// <summary>
    /// Controlador para validar y verificar las variables de entorno
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly ILogger<EnvironmentController> _logger;

        /// <summary>
        /// Constructor del controlador de entorno
        /// </summary>
        /// <param name="logger">Servicio de logging inyectado</param>
        public EnvironmentController(ILogger<EnvironmentController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el estado de las variables de entorno requeridas para la aplicación
        /// </summary>
        /// <returns>Estado de las variables de entorno y sus valores si está autorizado</returns>
        /// <response code="200">Devuelve el estado de las variables de entorno</response>
        /// <response code="401">No autorizado para ver información detallada</response>
        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateEnvironment()
        {
            try
            {
                // Lista de variables de entorno requeridas
                var requiredVariables = new List<string>
                {
                    "DEFAULT_CONNECTION",
                    "DIRECT_CONNECTION",
                    "JWT_SECRET",
                    "JWT_EXPIRY_MINUTES",
                    "LOGGING_ENABLED"
                };

                // Resultado de la validación
                var result = new
                {
                    IsRailway = IsRunningOnRailway(),
                    EnvFilePath = GetEnvFilePath(),
                    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "No definido",
                    Variables = requiredVariables.Select(v => new
                    {
                        Name = v,
                        IsPresent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(v)),
                        // Solo mostrar valores en entorno de desarrollo para seguridad
                        Value = IsDevelopment() ? MaskSensitiveValue(v, Environment.GetEnvironmentVariable(v)) : null
                    }).ToList()
                };

                // Verificar si todas las variables requeridas están presentes
                bool allVariablesPresent = result.Variables.All(v => v.IsPresent);

                // Registrar el resultado de la validación
                _logger.LogInformation($"Validación de variables de entorno: {(allVariablesPresent ? "Exitosa" : "Fallida")}");

                if (!allVariablesPresent)
                {
                    var missingVars = result.Variables.Where(v => !v.IsPresent).Select(v => v.Name);
                    _logger.LogWarning($"Variables de entorno faltantes: {string.Join(", ", missingVars)}");
                }

                return Ok(new
                {
                    Success = allVariablesPresent,
                    Message = allVariablesPresent
                        ? "Todas las variables de entorno requeridas están configuradas"
                        : "Faltan algunas variables de entorno requeridas",
                    Details = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar variables de entorno");
                return StatusCode(500, new { Success = false, Message = "Error al validar variables de entorno" });
            }
        }

        /// <summary>
        /// Verifica si la aplicación está ejecutándose en Railway
        /// </summary>
        /// <returns>True si se está ejecutando en Railway</returns>
        private bool IsRunningOnRailway()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));
        }

        /// <summary>
        /// Obtiene la ruta del archivo .env que se está utilizando
        /// </summary>
        /// <returns>Ruta del archivo .env o mensaje indicando que no se está utilizando</returns>
        private string GetEnvFilePath()
        {
            // Obtener la ruta que se utilizaría según la lógica de DotEnv
            var isRailway = IsRunningOnRailway();
            var envFilePath = DotEnv.FindEnvFile(isRailway);

            return System.IO.File.Exists(envFilePath)
                ? envFilePath
                : "Archivo .env no encontrado. Usando variables del sistema.";
        }

        /// <summary>
        /// Verifica si la aplicación está en entorno de desarrollo
        /// </summary>
        /// <returns>True si está en desarrollo</returns>
        private bool IsDevelopment()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return environment?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        /// <summary>
        /// Enmascara valores sensibles para mostrarlos de forma segura
        /// </summary>
        /// <param name="name">Nombre de la variable</param>
        /// <param name="value">Valor de la variable</param>
        /// <returns>Valor enmascarado si es sensible</returns>
        private string MaskSensitiveValue(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Lista de variables consideradas sensibles
            var sensitiveVars = new[] { "DEFAULT_CONNECTION", "DIRECT_CONNECTION", "JWT_SECRET" };

            if (sensitiveVars.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                // Mostrar solo los primeros 4 caracteres y enmascarar el resto
                if (value.Length > 4)
                {
                    return value.Substring(0, 4) + new string('*', value.Length - 4);
                }
                else
                {
                    return new string('*', value.Length);
                }
            }

            return value;
        }
    }
}
