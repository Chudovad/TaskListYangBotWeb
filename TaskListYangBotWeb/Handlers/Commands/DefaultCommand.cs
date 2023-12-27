using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class DefaultCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public DefaultCommand(TelegramBotService telegramBotHelper)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }
        public override string Name => CommandNames.DefaultCommand;

        public override async Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.GetTokenMsg + StaticFields.LinkToManual, parseMode: ParseMode.MarkdownV2);
        }
    }
}
