using BaseProject_DatabaseBeOn.Filters.AttributeAboutFitler;
using Microsoft.AspNetCore.Mvc.Filters;
namespace BaseProject_DatabaseBeOn.Filters.ResultFilters
{
    public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
    {

        //Rare to use => Always run result filter is rare to use
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                //Dieu kien khong thuc thi thang nay nhu da noi, no van vao filter nay nhung no se khong thuc thi tiep o day
                return;
            }
        }
    }
}
