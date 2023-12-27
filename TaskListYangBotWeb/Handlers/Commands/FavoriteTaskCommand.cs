using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TaskListYangBotWeb.Models;
using Telegram.Bot.Types.ReplyMarkups;
using TaskListYangBotWeb.Data.Interfaces;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class FavoriteTaskCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public FavoriteTaskCommand(TelegramBotService telegramBotHelper, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CommandNames.FavoriteTasksCommand;

        public override async Task ExecuteAsync(Update update)
        {
            List<FavoriteTask> favoriteTasks =_favoriteTaskRepository.GetUserFavoriteTasks(update.Message.Chat.Id).ToList();
            if (favoriteTasks.Count != 0)
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.RemoveMsg, replyMarkup: CreateButtons.GetButtonsFavoriteTasks(favoriteTasks));
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.RemoveMsg + StaticFields.FavoriteTaskMsg
                    ,replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Нет любимых заданий.");
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.FavoriteTaskMsg, replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });
            }
        }
    }
}
