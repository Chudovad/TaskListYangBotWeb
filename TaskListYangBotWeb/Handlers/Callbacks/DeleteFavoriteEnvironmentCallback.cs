using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class DeleteFavoriteEnvironmentCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public DeleteFavoriteEnvironmentCallback(TelegramBotService telegramBotHelper, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }
        public override string Name => CallbackNames.DeleteFavoriteEnvironmentCallback;

        public async override Task ExecuteAsync(Update update)
        {
            if (_favoriteEnvironmentRepository.DeleteFavoriteEnvironment(update.CallbackQuery.Message.Chat.Id, Convert.ToInt32(update.CallbackQuery.Data.Replace(CallbackNames.DeleteFavoriteEnvironmentCallback, ""))))
            {
                List<FavoriteEnvironment> favoriteEnvironments = _favoriteEnvironmentRepository.GetUserFavoriteEnvironments(update.CallbackQuery.Message.Chat.Id).ToList();
                await _telegramBotClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, 
                    messageId: update.CallbackQuery.Message.MessageId, 
                    replyMarkup: (InlineKeyboardMarkup)CreateButtons.GetButtonsFavoriteEnvironments(favoriteEnvironments));
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Ошибка удаления, повторите попытку");
        }
    }
}
