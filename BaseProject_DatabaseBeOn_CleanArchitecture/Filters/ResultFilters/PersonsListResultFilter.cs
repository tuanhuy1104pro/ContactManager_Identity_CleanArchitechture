using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseProject_DatabaseBeOn.Filters.ResultFilters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;
        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
        }


        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //TO DO before logic
            _logger.LogInformation($"{nameof(PersonsListResultFilter)}.{OnResultExecutionAsync} - Before");

             
            await next(); // View excuting here

            _logger.LogInformation($"{nameof(PersonsListResultFilter)}.{OnResultExecutionAsync} - After");

            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("dd/MMM/yyyy");

        }
    }
}
