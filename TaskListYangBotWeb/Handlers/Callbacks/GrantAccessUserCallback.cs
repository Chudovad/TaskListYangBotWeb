using Telegram.Bot;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class GrantAccessUserCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public GrantAccessUserCallback(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CallbackNames.GrantAccessCallback;

        public async override Task ExecuteAsync(Update update)
        {

        }
    }
}
