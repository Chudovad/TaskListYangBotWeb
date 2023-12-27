using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;
using TaskListYangBotWeb.Data.Interfaces;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class CheckTokenCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public CheckTokenCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.CheckTokenCommand;

        public override async Task ExecuteAsync(Update update)
        {
            if (ParseYangService.RequestToApiCheckToken(update.Message.Text).message == null)
            {
                if (_userRepository.UpdateUserToken(update.Message.Chat.Id, update.Message.Text))
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Токен записан", replyToMessageId: update.Message.MessageId);
                else
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Токен не записан, повторите попытку", replyToMessageId: update.Message.MessageId);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "Неверный токен" + StaticFields.LinkToManual,
                    replyToMessageId: update.Message.MessageId, parseMode: ParseMode.MarkdownV2);
            }
        }
    }
}
