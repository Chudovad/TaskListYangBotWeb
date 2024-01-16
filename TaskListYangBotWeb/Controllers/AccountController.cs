using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        private IMessageRepository _messageRepository;
        private IFavoriteTaskRepository _favoriteTaskRepository;

        public AccountController(IUserRepository userRepository, IMessageRepository messageRepository, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _favoriteTaskRepository = favoriteTaskRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            return View(_userRepository.GetUsers());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{controller}/{action}/{userId:int}")]
        public IActionResult GetUserMessages(int userId)
        {
            return View(_messageRepository.GetUserMessages(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetMessages()
        {
            return View(_messageRepository.GetMessages());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{controller}/{action}/{userId:long}")]
        public IActionResult GetUserFavoriteTasks(long userId)
        {
            return View(_favoriteTaskRepository.GetUserFavoriteTasks(userId));
        }
    }
}
