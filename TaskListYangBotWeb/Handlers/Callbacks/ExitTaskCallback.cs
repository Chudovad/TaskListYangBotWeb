using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
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
        public override string Name => CallbackNames.TypeSortingCallback;

        public async override Task ExecuteAsync(Update update)
        {
            string tokenYang = StaticFields.GetUserToken(TelegramBotHelper.GetChatId(update));
            List<Root> taskList = ParseYang.RequestToApiTaskList(tokenYang);
            List<Root> leaveTask = taskList.Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            ParseYang.RequestToApiLeaveTask(leaveTask[0].pools[0].activeAssignments[0].id, tokenYang);
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Вышли");
            await client.SendTextMessageAsync(TelegramBotHelper.GetChatId(update),
                    "Вышли из задания " + leaveTask[0].description);
        }
    }
}
