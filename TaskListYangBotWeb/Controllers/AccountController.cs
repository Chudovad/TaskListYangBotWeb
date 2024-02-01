using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;

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
        public IActionResult GetUserMessages(int userId, int page = 1, int pageSize = 10)
        {
            var userMessages = _messageRepository.GetUserMessages(userId);

            PageViewModel viewModel = GetPage(userMessages, page, pageSize);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetMessages(int page = 1, int pageSize = 10)
        {
            var messagesData = _messageRepository.GetMessages();

            PageViewModel viewModel = GetPage(messagesData, page, pageSize);

            return View(viewModel);
        }

        private PageViewModel GetPage(ICollection<Message> data, int page, int pageSize)
        {
            int totalMessages = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalMessages / pageSize);

            var currentPageData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new PageViewModel
            {
                Messages = currentPageData,
                PageInfoModel = new PageInfoModel
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalMessages,
                    TotalPages = totalPages
                }
            };
            return viewModel;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{controller}/{action}/{userId:long}")]
        public IActionResult GetUserFavoriteTasks(long userId)
        {
            return View(_favoriteTaskRepository.GetUserFavoriteTasks(userId));
        }
    }
}
