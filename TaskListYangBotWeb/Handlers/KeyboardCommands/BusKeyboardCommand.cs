using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;

namespace TaskListYangBotWeb.Handlers.KeyboardCommands
{
    public class BusKeyboardCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public BusKeyboardCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => KeyboardCommandNames.BusKeyboard;

        public async override Task ExecuteAsync(Update update)
        {

        }
    }
}
