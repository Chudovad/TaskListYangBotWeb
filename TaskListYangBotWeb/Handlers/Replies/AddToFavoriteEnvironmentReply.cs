using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Replies
{
    public class AddToFavoriteEnvironmentReply : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public AddToFavoriteEnvironmentReply(TelegramBotService telegramBotHelper, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }

        public override string Name => ReplayNames.AddToFavoriteEnvironmentReply;

        public async override Task ExecuteAsync(Update update)
        {
            if (_favoriteEnvironmentRepository.AddFavoriteEnvironment(update.Message.Chat.Id, update.Message.Text, new Random().Next(1000000)))
            {
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId);
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId - 1);
                await _telegramBotClient.DeleteMessageAsync(update.Message.Chat.Id, messageId: update.Message.MessageId - 2);
                List<FavoriteEnvironment> favoriteEnvironments = _favoriteEnvironmentRepository.GetUserFavoriteEnvironments(update.Message.Chat.Id).ToList();
                if (favoriteEnvironments.Count != 0)
                {
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.RemoveEnvironmentMsg, replyMarkup: CreateButtons.GetButtonsFavoriteEnvironments(favoriteEnvironments));
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.FavoriteEnvironmentMsg, 
                        replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название окружения" });
                }
                else
                {
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Нет любимых окружений.");
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.FavoriteEnvironmentMsg, 
                        replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "Название окружения" });
                }
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Задание уже есть в списке окружений");
        }
    }
}
