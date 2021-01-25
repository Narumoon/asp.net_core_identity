﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Udemy.Models;

namespace Udemy.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<UserModel> _userManager;


        private readonly IUserClaimsPrincipalFactory<UserModel> _userClaimsPrincipalFactory;
        private readonly SignInManager<UserModel> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<UserModel> userManager,
            IUserClaimsPrincipalFactory<UserModel> userClaimsPrincipalFactory,
            SignInManager<UserModel> signInManager) 
        {
            _logger = logger;
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByNameAsync(model.UserName);

                //if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                //{
                //    var identity = new ClaimsIdentity("Identity.Applicaton");
                //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                //    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                //    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                //    await HttpContext.SignInAsync("Identity.Application", principal);

                //    return RedirectToAction("About");
                //}

                var signInResult = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("About");
                }

                ModelState.AddModelError("", "Invalid password!");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if(user == null)
                {
                    user = new UserModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);


                }
                return View("Success");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel { Token = token, Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(
                        user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View();
                    }
                    return View("Success");

                }

                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var resetURL = Url.Action("ResetPassword", "Home", 
                        new {
                            token = token,
                            email = model.Email
                        }, Request.Scheme);

                    System.IO.File.WriteAllText("resetLink.txt", resetURL);

                    return View("Success");
                }
                else
                {
                    return View();
                }
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
