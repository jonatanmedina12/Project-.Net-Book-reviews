using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del controlador de entorno
        /// </summary>
        /// <param name="logger">Servicio de logging inyectado</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public EnvironmentController(ILogger<EnvironmentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene el estado de las variables de entorno requeridas para la aplicación
        /// </summary>
        /// <returns>Estado de las variables de entorno y sus valores si está autorizado</returns>
        /// <response code="200">Devuelve el estado de las variables de entorno</response>
        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

                // Mapeo de variables de entorno a configuración
                var configMapping = new Dictionary<string, string>
                {
                    { "DEFAULT_CONNECTION", "ConnectionStrings:DefaultConnection" },
                    { "DIRECT_CONNECTION", "ConnectionStrings:DirectConnection" },
                    { "JWT_SECRET", "JWT:Secret" },
                    { "JWT_EXPIRY_MINUTES", "JWT:ExpiryInMinutes" },
                    { "LOGGING_ENABLED", "Logging:Enabled" }
                };

                // Comprobar primero valores directos de Environment.GetEnvironmentVariable
                var envVarsStatus = requiredVariables.Select(v => new
                {
                    Name = v,
                    EnvVarPresent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(v)),
                    EnvVarValue = MaskSensitiveValue(v, Environment.GetEnvironmentVariable(v))
                }).ToList();

                // Comprobar valores en la configuración
                var configVarsStatus = configMapping.Select(mapping => new
                {
                    Name = mapping.Key,
                    ConfigKey = mapping.Value,
                    ConfigPresent = !string.IsNullOrEmpty(_configuration[mapping.Value]),
                    ConfigValue = MaskSensitiveValue(mapping.Key, _configuration[mapping.Value])
                }).ToList();

                // Resultado de la validación
                var result = new
                {
                    IsRailway = IsRunningOnRailway(),
                    EnvVariables = envVarsStatus,
                    ConfigVariables = configVarsStatus,
                    ApplicationConfigured = configVarsStatus.All(v => v.ConfigPresent)
                };

                // Verificar si todas las variables requeridas están presentes en la configuración
                bool allVariablesPresent = result.ApplicationConfigured;

                // Registrar el resultado de la validación
                _logger.LogInformation($"Validación de variables de entorno: {(allVariablesPresent ? "Exitosa" : "Fallida")}");

                if (!allVariablesPresent)
                {
                    var missingVars = configVarsStatus.Where(v => !v.ConfigPresent).Select(v => v.Name);
                    _logger.LogWarning($"Variables de configuración faltantes: {string.Join(", ", missingVars)}");
                }

                return Ok(new
                {
                    Success = allVariablesPresent,
                    Message = allVariablesPresent
                        ? "Todas las variables de entorno requeridas están configuradas correctamente"
                        : "Faltan algunas variables de configuración requeridas",
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
        /// Enmascara valores sensibles para mostrarlos de forma segura
        /// </summary>
        /// <param name="name">Nombre de la variable</param>
        /// <param name="value">Valor de la variable</param>
        /// <returns>Valor enmascarado si es sensible</returns>
        private string MaskSensitiveValue(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return "[vacío]";

            // Lista de variables consideradas sensibles
            var sensitiveVars = new[] { "DEFAULT_CONNECTION", "DIRECT_CONNECTION", "JWT_SECRET" };

            if (sensitiveVars.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                // Mostrar solo los primeros 4 caracteres y enmascarar el resto
                if (value.Length > 4)
                {
                    return value.Substring(0, 4) + new string('*', 8);
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