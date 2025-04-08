using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BookReviews.API.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ArgumentException), HandleArgumentException },
                { typeof(ArgumentNullException), HandleArgumentNullException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(InvalidOperationException), HandleInvalidOperationException }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleArgumentException(ExceptionContext context)
        {
            var exception = (ArgumentException)context.Exception;
            _logger.LogError(exception, "Argumento inválido: {Message}", exception.Message);

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud inválida",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private void HandleArgumentNullException(ExceptionContext context)
        {
            var exception = (ArgumentNullException)context.Exception;
            _logger.LogError(exception, "Argumento nulo: {Message}", exception.Message);

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud inválida",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var exception = (UnauthorizedAccessException)context.Exception;
            _logger.LogError(exception, "Acceso no autorizado: {Message}", exception.Message);

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = exception.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            context.ExceptionHandled = true;
        }

        private void HandleInvalidOperationException(ExceptionContext context)
        {
            var exception = (InvalidOperationException)context.Exception;
            _logger.LogError(exception, "Operación inválida: {Message}", exception.Message);

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Operación inválida",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ha ocurrido un error",
                Detail = "Se ha producido un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
