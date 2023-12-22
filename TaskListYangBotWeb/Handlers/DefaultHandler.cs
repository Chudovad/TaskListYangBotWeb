using TaskListYangBotWeb.Helper;
using TaskListYangTgBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskListYangBotWeb.Handlers
{
    public class DefaultHandler : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public DefaultHandler(TelegramBotService telegramBotHelper)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }
        public override string Name => "Default";

        public override async Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.GetTokenMsg + StaticFields.LinkToManual, parseMode: ParseMode.MarkdownV2);
        }
    }
}
