using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseProject_DatabaseBeOn.Filters
{
    public class ResponeHeaderActionFilter : IActionFilter , IOrderedFilter
    {
        private readonly ILogger<ResponeHeaderActionFilter> _logger;
        private readonly string Key, Value;
        public int Order { get; set; }
        public ResponeHeaderActionFilter(ILogger<ResponeHeaderActionFilter> logger,string key,string value,int order)
        {

            _logger = logger;
            Key = key;
            Value = value;
            Order = order; // nó tự Order cho ta luôn . Ngon
        }

        

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"{nameof(ResponeHeaderActionFilter)}.{nameof(OnActionExecuted)} method");
            context.HttpContext.Response.Headers[Key] = Value;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"{nameof(ResponeHeaderActionFilter)}.{nameof(OnActionExecuting)} method");
            
        }
    }
}

//////// ResultFilter It xai trong real project, sometime resource Filter luôn
///CÒn lại thì dùng nhiều (authentication - exception - ation) filter