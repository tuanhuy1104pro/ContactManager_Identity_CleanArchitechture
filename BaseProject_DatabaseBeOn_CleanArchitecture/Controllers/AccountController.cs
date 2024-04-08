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
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _usermanager = userManager;
            _signInManager = signInManager;
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
                /////THường đăng ký xong sẽ đăng nhập luôn => lí do đấy, lúc này sv sẽ đưa cho client cookie chứa thông tin đăng nhập user ---------Login
                ///
                await _signInManager.SignInAsync(user, isPersistent: false); // lúc này sẽ cấp cookie

                //Login

                // redirect view
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

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }    

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Persons");
        }
    }
}
