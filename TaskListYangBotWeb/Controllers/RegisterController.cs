using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;

namespace TaskListYangBotWeb.Controllers
{
    public class RegisterController : Controller
    {
        private IConfiguration _configuration;
        private IUserWebRepository _userWebRepository;

        public RegisterController(IConfiguration configuration, IUserWebRepository userWebRepository)
        {
            _configuration = configuration;
            _userWebRepository = userWebRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index([FromForm] UserWebRegister user, int roleId)
        {
            if (!ModelState.IsValid)
                return View(user);

            if (_userWebRepository.CreateUser(user, roleId))
            {
                var token = EncryptionService.GenerateJwt(_userWebRepository.GetUser(user.Username), _configuration, out CookieOptions cookieOptions);
                Response.Cookies.Append("AuthToken", token, cookieOptions);
                return RedirectToAction(nameof(AccountController.Index), "Account");
            }
            else
            {
                ModelState.AddModelError("Username", "User exists");
                return View(user);
            }
        }
    }
}
