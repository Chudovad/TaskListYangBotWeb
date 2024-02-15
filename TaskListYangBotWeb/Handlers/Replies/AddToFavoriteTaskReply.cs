using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Handlers.Callbacks;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Handlers.Commands;
using TaskListYangBotWeb.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Replies
{
    public class AddToFavoriteTaskReply : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public AddToFavoriteTaskReply(TelegramBotService telegramBotHelper, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteTaskRepository = favoriteTaskRepository;
        }

        public override string Name => ReplayNames.AddToFavoriteTaskReply;

        public async override Task ExecuteAsync(Update update)
        {
            if (_favoriteTaskRepository.AddFavoriteTask(update.Message.Chat.Id, update.Message.Text, new Random().Next(1000000)))
            {
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId);
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId - 1);
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId - 2);
                List<FavoriteTask> favoriteTasks = _favoriteTaskRepository.GetUserFavoriteTasks(update.Message.Chat.Id).ToList();
                if (favoriteTasks.Count != 0)
                {
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.RemoveTaskMsg, replyMarkup: CreateButtons.GetButtonsFavoriteTasks(favoriteTasks));
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.FavoriteTaskMsg
                        , replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });
                }
                else
                {
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Нет любимых заданий.");
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.FavoriteTaskMsg, replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название задания" });
                }
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Задание уже есть в списке любимых");
        }
    }
}
