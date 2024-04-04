using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseProject_DatabaseBeOn.Filters
{
    public class LearnAsyncFilter : IAsyncActionFilter, IOrderedFilter
    {
        public int Order {  get; set; }
        private readonly string _Key;
        private readonly string _Value;
        private readonly ILogger<LearnAsyncFilter> _logger;

        public LearnAsyncFilter(ILogger<LearnAsyncFilter> Logger,string key,string value,int order)
        {
            Order = order;
            _Key = key;
            _Value = value;
            _logger = Logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponeHeaderActionFilter), nameof(OnActionExecutionAsync));
            
            await next(); //calls the subsequent filter or action method

            _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponeHeaderActionFilter), nameof(OnActionExecutionAsync));

            context.HttpContext.Response.Headers[_Key] = _Value;


            //TO DO: after logic here
        }
    }
}

//////AsyncFilter => CÓ thể call database hay các service Async method khác => Context Có tất cả method mà Excuted hay excuting ờ non-async filter có

//Ca dao, trước next là excuting sau next là excuted :))


