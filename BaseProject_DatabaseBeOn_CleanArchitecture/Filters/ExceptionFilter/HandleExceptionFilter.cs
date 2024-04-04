using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Filters.ExceptionFilter
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IHostEnvironment _environment;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;

        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError($"Exception Filter {nameof(HandleExceptionFilter)}.{nameof(OnException)}\nExceptionType:{context.Exception.GetType().ToString()}\nExceptionMessage: {context.Exception.Message}");

            //context.Result = new RedirectToActionResult(); Page mà ta muốn result respone view cho browser => Result luôn luôn Circicuting các tiến trình kế tiếp của pieline

            ////// thật sự thì thằng này hên xui hay chỉ dùng cho phạm vi nhỏ các action thì xài chứ  người ta vẫn thích dùng "Error handle middleware hơn"
            ///
            if (_environment.IsDevelopment())
            {
                context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = 500 };
            }

        }
    }
}
