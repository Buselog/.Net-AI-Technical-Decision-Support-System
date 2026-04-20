using RepairGuidance.Domain.Exceptions;
using Serilog;
using System.Data;
using System.Net;
using System.Text.Json;

namespace RepairGuidance.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorMessage) = exception switch
            {
                FluentValidation.ValidationException valEx =>
                  (HttpStatusCode.BadRequest, string.Join(" | ", valEx.Errors.Select(e => e.ErrorMessage))),

                UserNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                StepNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                DeviceNotEligibleException => (HttpStatusCode.BadRequest, exception.Message),

                AiServiceException => (HttpStatusCode.BadGateway, exception.Message),

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Bu işlem için yetkiniz bulunmamakta."),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                _ => (HttpStatusCode.InternalServerError, "Sistem kaynaklı bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.")
            };


            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Kritik Sistem Hatası: {Message}", exception.Message);
            else
                Log.Warning(exception, "İş Mantığı İhlali: {Message}", errorMessage);

            var response = new
            {
                Status = context.Response.StatusCode,
                Message = errorMessage,
                Detail = exception.GetType().Name,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
