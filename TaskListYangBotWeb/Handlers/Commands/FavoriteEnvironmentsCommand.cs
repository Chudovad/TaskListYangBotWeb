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
    public class FavoriteEnvironmentsCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public FavoriteEnvironmentsCommand(TelegramBotService telegramBotHelper, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }
        public override string Name => CommandNames.FavoriteEnvironmentsCommand;

        public override async Task ExecuteAsync(Update update)
        {
            List<FavoriteEnvironment> favoriteEnvironments = _favoriteEnvironmentRepository.GetUserFavoriteEnvironments(update.Message.Chat.Id).ToList();
            if (favoriteEnvironments.Count != 0)
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.RemoveEnvironmentMsg, 
                    replyMarkup: CreateButtons.GetButtonsFavoriteEnvironments(favoriteEnvironments));
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
    }
}
