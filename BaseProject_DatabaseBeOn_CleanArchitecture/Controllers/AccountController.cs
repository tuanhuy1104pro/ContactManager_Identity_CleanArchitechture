using BaseProject_DatabaseBeOn.Controllers;
using CoreLayer.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Controllers
{
    [Route("[Controller]/[Action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            return RedirectToAction(nameof(PersonsController.Index),"Persons");
        }
    }
}
