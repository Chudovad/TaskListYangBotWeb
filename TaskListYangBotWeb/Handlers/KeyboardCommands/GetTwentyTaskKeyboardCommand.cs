using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.KeyboardCommands
{
    public class GetTwentyTaskKeyboardCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public GetTwentyTaskKeyboardCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => KeyboardCommandNames.GetTwentyTaskKeyboard;

        public async override Task ExecuteAsync(Update update)
        {
            await ParseYangService.GetNextPageTaskList(update, _telegramBotClient, _userRepository, 20);
        }

    }
}
