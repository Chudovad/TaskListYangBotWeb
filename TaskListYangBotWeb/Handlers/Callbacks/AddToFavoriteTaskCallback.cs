using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class AddToFavoriteTaskCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public AddToFavoriteTaskCallback(TelegramBotService telegramBotHelper, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CallbackNames.AddToFavoriteTaskCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            var task = CommandStatus.taskListsUsers[update.CallbackQuery.Message.Chat.Id].Where(x => (int)x.pools[0].id == Convert.ToInt32(update.CallbackQuery.Data.Replace(Name, ""))).ToList();
            string taskName = ParseYangService.GetTaskNameInDescription((string)task[0].description);
            if (_favoriteTaskRepository.AddFavoriteTask(update.CallbackQuery.Message.Chat.Id, taskName, (long)task[0].pools[0].id))
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Добавили задание '" + taskName + "' в любимые");
            else
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Задание уже есть в списке любимых");
        }
    }
}
