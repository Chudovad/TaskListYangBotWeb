using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class AddToFavoriteEnvironmentCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public AddToFavoriteEnvironmentCallback(TelegramBotService telegramBotHelper, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }
        public override string Name => CallbackNames.AddToFavoriteEnvironmentCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            var task = CommandStatus.taskListsUsers[update.CallbackQuery.Message.Chat.Id].Where(x => (int)x.pools[0].id == Convert.ToInt32(update.CallbackQuery.Data.Replace(Name, ""))).ToList();
            string environmentName = ParseYangService.GetEnvironmentInDescription((string)task[0].description);
            if (_favoriteEnvironmentRepository.AddFavoriteEnvironment(update.CallbackQuery.Message.Chat.Id, environmentName, (long)task[0].pools[0].id))
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Добавили окружение '" + environmentName + "' в любимые");
            else
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Окружение уже есть в списке любимых");
        }
    }
}
