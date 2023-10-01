using System.Text.Json;
using Application.Common.Exceptions;
using FluentValidation;

namespace Api.Middleware
{

    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private static async Task HandleException(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var response = new ExceptionMessage
            {
                StatusCode = statusCode,
                Message = exception.Message,
                Errors = GetErrors(exception)
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        
        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                AppException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbidException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

        private static List<ErrorMessage> GetErrors(Exception exception)
        {
            var errors = new List<ErrorMessage>();
            if (exception is ValidationException validationException)
            {
                errors = validationException.Errors.Select(x => new ErrorMessage
                    { PropertyName = x.PropertyName, Details = x.ErrorMessage }).ToList();
            }
            return errors;
        }
    }

}