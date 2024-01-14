using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;

namespace TaskListYangBotWeb.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _configuration;
        private IUserWebRepository _userWebRepository;

        public LoginController(IConfiguration configuration, IUserWebRepository userWebRepository)
        {
            _configuration = configuration;
            _userWebRepository = userWebRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index([FromForm] UserWebLogin user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var currentUser = _userWebRepository.AuthenticateUser(user);

            if (currentUser != null)
            {
                var token = EncryptionService.GenerateJwt(currentUser, _configuration, out CookieOptions cookieOptions);

                Response.Cookies.Append("AuthToken", token, cookieOptions);
                return RedirectToAction(nameof(AccountController.Index), "Account");
            }
            else
            {
                ModelState.AddModelError("Username", "Username or password incorrect");
                return View(user);
            }

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
