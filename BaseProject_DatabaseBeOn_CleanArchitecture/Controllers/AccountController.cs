using BaseProject_DatabaseBeOn.Controllers;
using CoreLayer.Domain.IdentityEntities;
using CoreLayer.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Controllers
{   
    [Route("[Controller]/[Action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        public AccountController(UserManager<ApplicationUser> usermanager)
        {
            _usermanager = usermanager;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(registerDTO);
            }
            ApplicationUser user = new ApplicationUser() {Email = registerDTO.Email,PhoneNumber = registerDTO.Phone,UserName = registerDTO.Email,PersonName = registerDTO.PersonName };
            IdentityResult result =  await _usermanager.CreateAsync(user,registerDTO.Password);
            //IdentityResult contain status of identity
            
            if(result.Succeeded)
            {
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register",error.Description);
                }
                return View(registerDTO);
            }
            // Store usser registration details into Identity database

           
        }
    }
}
