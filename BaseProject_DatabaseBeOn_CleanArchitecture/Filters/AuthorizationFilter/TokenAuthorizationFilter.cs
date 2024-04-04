using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BaseProject_DatabaseBeOn.Controllers;
namespace BaseProject_DatabaseBeOn.Filters.AuthorizationFilter
{
    public class TokenAuthorizationFilter : IAsyncAuthorizationFilter
    {


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
           if(context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }
            if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
            //Tạm thời hiểu là vậy, cách tạo cookie hay gì gì đó thì chờ học tới identity
        }
    }
}
