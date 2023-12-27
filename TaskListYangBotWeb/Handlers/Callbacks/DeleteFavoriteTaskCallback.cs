using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class DeleteFavoriteTaskCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public DeleteFavoriteTaskCallback(TelegramBotService telegramBotHelper, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CallbackNames.DeleteFavoriteTaskCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");

            if (_favoriteTaskRepository.DeleteFavoriteTask(update.CallbackQuery.Message.Chat.Id, Convert.ToInt32(update.CallbackQuery.Data.Replace(CallbackNames.DeleteFavoriteTaskCallback, ""))))
            {
                List<FavoriteTask> favoriteTasks = _favoriteTaskRepository.GetUserFavoriteTasks(update.CallbackQuery.Message.Chat.Id).ToList();
                await _telegramBotClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, 
                    messageId: update.CallbackQuery.Message.MessageId, 
                    replyMarkup: (InlineKeyboardMarkup)CreateButtons.GetButtonsFavoriteTasks(favoriteTasks));
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Ошибка удаления, повторите попытку");
        }
    }
}
