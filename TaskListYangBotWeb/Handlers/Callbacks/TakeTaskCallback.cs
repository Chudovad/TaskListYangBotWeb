using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class TakeTaskCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public TakeTaskCallback(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }

        public override string Name => CallbackNames.TakeTaskCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            var userToken = _userRepository.GetUserToken(update.CallbackQuery.Message.Chat.Id);
            var takeTaskResponse = ParseYangService.RequestToApiTakeTask(update.CallbackQuery.Data.Replace(Name, ""), userToken);
            ParseYangService.GetMessageTakingTask(takeTaskResponse, _telegramBotClient, update.CallbackQuery.Message.Chat.Id);
        }
    }
}
