using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class ExitTaskCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public ExitTaskCallback(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CallbackNames.ExitTaskCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            string tokenYang = _userRepository.GetUserToken(update.CallbackQuery.Message.Chat.Id);
            List<dynamic> taskList = ParseYangService.RequestToApiTaskList(tokenYang);
            List<dynamic> leaveTask = taskList.Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            ParseYangService.RequestToApiLeaveTask((string)leaveTask[0].pools[0].activeAssignments[0].id, tokenYang);
            await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                    $"Вышли из задания '{(string)leaveTask[0].description}'");
        }
    }
}
