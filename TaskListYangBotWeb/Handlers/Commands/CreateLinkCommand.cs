using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class CreateLinkCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public CreateLinkCommand(TelegramBotService telegramBotHelper)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }

        public override string Name => CommandNames.CreateLinkCommand;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"`{update.Message.Text}`",
                replyToMessageId: update.Message.MessageId,
                parseMode: ParseMode.MarkdownV2);
        }
    }
}
