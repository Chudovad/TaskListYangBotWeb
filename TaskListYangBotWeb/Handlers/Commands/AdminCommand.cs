using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class AdminCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AdminCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository, IConfiguration configuration)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public override string Name => CommandNames.AdminCommand;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(
                update.Message.Chat.Id,
                "Открыть админ-панель:",
                replyMarkup: CreateButtons.GetButtonWebApp("Админ-панель", _configuration["UrlWebhook"])
            );
        }
    }
}
