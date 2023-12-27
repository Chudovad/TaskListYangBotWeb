using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class AddToFavoriteCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public AddToFavoriteCallback(TelegramBotService telegramBotHelper, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CallbackNames.AddToFavoriteCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            var task = CommandStatus.taskListsUsers[update.CallbackQuery.Message.Chat.Id].Where(x => (int)x.pools[0].id == Convert.ToInt32(update.CallbackQuery.Data.Replace(Name, ""))).ToList();
            if (_favoriteTaskRepository.AddFavoriteTask(update.CallbackQuery.Message.Chat.Id, (string)task[0].description, (long)task[0].pools[0].id))
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Добавили задание '" + (string)task[0].description + "' в любимые");
            else
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Задание уже есть в списке любимых");
        }
    }
}
