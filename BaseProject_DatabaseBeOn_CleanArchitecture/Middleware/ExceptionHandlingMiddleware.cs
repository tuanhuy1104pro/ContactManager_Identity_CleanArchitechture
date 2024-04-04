using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BaseProject_DatabaseBeOn.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

           
            try
            {
               await  _next(httpContext);
            }
            catch (Exception ex)
            {
                // Các exption từ các middleware khác sẽ được catch ở đây, do thằng middleware này được thực thi sớm nhất
                if(ex.InnerException != null)
                {
                    _logger.LogError("{ExeptionType} {ExceptionMessage}",ex.InnerException.GetType().ToString(),ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExeptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
                }
               // httpContext.Response.StatusCode = 500;
               //await  httpContext.Response.WriteAsync("Error occcurred"); => comment lại vì nó sẽ không respone về browser nữa, nhiệm vụ này sẽ cho thằng buildin lo

                throw; // rethrow và thằng buildin "UseExceptionHandler sẽ catch lấy,
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
