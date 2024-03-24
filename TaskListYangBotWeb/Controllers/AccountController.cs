using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public AccountController(IUserRepository userRepository, IMessageRepository messageRepository, IFavoriteTaskRepository favoriteTaskRepository, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _favoriteTaskRepository = favoriteTaskRepository;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
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
        public IActionResult GetUserMessages(int id, int page = 1, int pageSize = 10)
        {
            var userMessages = _messageRepository.GetUserMessages(id);

            PageViewModel viewModel = GetPage(userMessages, page, pageSize);
            ViewData["Action"] = nameof(GetUserMessages);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetMessages(int page = 1, int pageSize = 10)
        {
            var messagesData = _messageRepository.GetMessages();

            PageViewModel viewModel = GetPage(messagesData, page, pageSize);
            ViewData["Action"] = nameof(GetMessages);

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
        public IActionResult GetUserFavoriteTasks(long id)
        {
            return View(_favoriteTaskRepository.GetUserFavoriteTasks(id));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetUserFavoriteEnvironments(long id)
        {
            return View(_favoriteEnvironmentRepository.GetUserFavoriteEnvironments(id));
        }
    }
}
