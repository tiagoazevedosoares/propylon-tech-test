using System;
using System.Threading.Tasks;
using AutoMapper;
using DTO.Users;
using Frontend.Extensions;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAPIService apiService;
        private readonly IMapper mapper;

        public AccountController(IAPIService apiService, IMapper mapper)
        {
            this.apiService = apiService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new UserLoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await apiService.Login(mapper.Map<AuthenticateModel>(model));

                HttpContext.Session.Set<UserModelWithToken>("UserWithToken", user);

                return RedirectToAction("index", "documents");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Login", ex.Message);

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new UserRegistrationViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await apiService.Register(mapper.Map<RegisterModel>(model));

                return RedirectToAction("login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Login", ex.Message);

                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Set<DTO.Users.UserModelWithToken>("UserWithToken", null);

            return RedirectToAction("login", "account");
        }
    }
}
