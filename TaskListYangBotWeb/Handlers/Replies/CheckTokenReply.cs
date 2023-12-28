using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Replies
{
    public class CheckTokenReply : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public CheckTokenReply(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => ReplayNames.CheckTokenReply;

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
                    "Неверный токен.",
                    replyToMessageId: update.Message.MessageId);
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.GetTokenMsg,
                    replyMarkup: new ForceReplyMarkup { Selective = true, InputFieldPlaceholder = "OAuth токен" },
                    parseMode: ParseMode.MarkdownV2);
            }
        }
    }
}
