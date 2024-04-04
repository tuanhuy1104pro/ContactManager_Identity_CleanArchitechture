using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseProject_DatabaseBeOn.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isDisabled;
        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger,bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;

        }


        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //To DO: before logic
            _logger.LogInformation($"{nameof(FeatureDisabledResourceFilter)} : {nameof(OnResourceExecutionAsync)} excuting");
            if(_isDisabled)
            {
                 context.Result = new NotFoundResult();
            }
            else
            {
                await next(); // Dùng result rồi thì không nên call next() phải đặt next ở else, một trong hai thằng chỉ nên xuất hiện duy nhất
               
            }
            _logger.LogInformation($"{nameof(FeatureDisabledResourceFilter)} : {nameof(OnResourceExecutionAsync)} excuted");

        }
    }
}
