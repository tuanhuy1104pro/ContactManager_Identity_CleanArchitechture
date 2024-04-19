using BaseProject_DatabaseBeOn.Controllers;
using CoreLayer.Domain.IdentityEntities;
using CoreLayer.DTO;
using CoreLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Controllers
{   
    [Route("[Controller]/[Action]")]
    [AllowAnonymous] // một số trường hợp thằng này khá là báo vì cho truy cập hết
    public class AccountController : Controller
    {
       
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _rolemanager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager)
        {
            _usermanager = userManager;
            _signInManager = signInManager;
            _rolemanager = rolemanager;
        }
        [HttpGet]
        [Authorize("NotAuthenticated")] //Custom policy tạo ở program
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
        [ValidateAntiForgeryToken] // Điều kiện ở form phải dùng tag helper hết mới xài được => Only for Post action nhá
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
                //Check status of radio button
                if(registerDTO.UserType == UserTypeOptions.Admin )
                {
                    //Create 'Admin' role 
                    if(await _rolemanager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.Admin.ToString() };
                        await _rolemanager.CreateAsync(applicationRole);
                    }


                    //Add the new user into 'Admin' role
                   await _usermanager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                }
                else
                {
                    //Create 'User' role 
                    if (await _rolemanager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.User.ToString() };
                        await _rolemanager.CreateAsync(applicationRole);
                    }


                    //Add the new user into 'User' role
                    await _usermanager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                }


                //

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
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginDTO login,string? ReturnUrl)
        {

            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(login);
            }
           var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false,lockoutOnFailure:false); //lockoutOnFailure true nếu người dùng nhập sai nhiều quá nó sẽ lock
            
            if(result.Succeeded )
            {
               ApplicationUser user = await _usermanager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    if (await _usermanager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Home",new {area = "Admin"});
                    }    
                }
               
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return LocalRedirect(ReturnUrl);
                else
                return RedirectToAction("Index", "Persons");
            }
            else
            {
                ModelState.AddModelError("Login","Invalid email or password");
                return View(login);
            }
        }
        [Authorize] // Kiểu này thì là bất kể role nào miễn là đăng nhập đều xài được trái ngược với AllowAnonymous
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Persons");
        }
        public async Task<IActionResult> isEmailAlreadyHave(string email)
        {
            ApplicationUser user = await _usermanager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true); //valid -> mean email already have
            }
            else
            {
                return Json(false);////invalid
            }


            /////// Cai nay dung Remote Validation nhung, tam thoi chua tim duoc package tuong ung =
            /// => dùng cách khác cho lành ý tưởng => viết services => apply vào controller, nếu có lỗi thì quăng ra viewbag, khỏi cần xử lý ở build-in chi cho nặng
        }

        public  async Task<IActionResult> About()
        {

            var user =  _usermanager.GetUserAsync(User).Result;
            IList<String> role = await _usermanager.GetRolesAsync(user); // Một ví dụ về Cast, thằng GetRolesAsync trả về kiểu task<ilist<string>> tức là phải dùng await xong thì nó mới trả về ilist<string>. Task ở dây hiểu là phải chờ nó thực hiện xong thì mới return giá trị tương ứng. Không có await được hiểu sẽ trả về task<ilist<string>> (bởi nhiều khi nó chưa hoạt động xong mà đã gán cho user rồi)
            ViewBag.Role = (IEnumerable<string>) role;

            return View( await GetCurrentUser());
        }
        public async Task<ApplicationUser> GetCurrentUser()
        {
            //ClaimsPrincipal currentUser = User; test
            var user =  _usermanager.GetUserAsync(User).Result;
            
            return  user;
        }
    }
}
