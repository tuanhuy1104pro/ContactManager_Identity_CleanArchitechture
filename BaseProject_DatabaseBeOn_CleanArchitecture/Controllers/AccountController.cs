﻿using BaseProject_DatabaseBeOn.Controllers;
using CoreLayer.Domain.IdentityEntities;
using CoreLayer.DTO;
using CoreLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject_DatabaseBeOn_CleanArchitecture.Controllers
{   
    [Route("[Controller]/[Action]")]
    [AllowAnonymous]
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
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
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
            //Nghiên cứu sau:
           
            return View();
        }    
    }
}
