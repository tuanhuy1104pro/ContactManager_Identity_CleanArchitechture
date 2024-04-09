using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,User")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        //[Route("Admin/[controller]/[action]")]
        [Route("Admin/Home/Index")]
        //[Authorize(Roles = "Admin,User")] cho phép admin và user
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
