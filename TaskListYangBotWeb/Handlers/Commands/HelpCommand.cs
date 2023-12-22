using Telegram.Bot;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangTgBot;
using Telegram.Bot.Types.Enums;

namespace TaskListYangBotWeb.Handlers.Commands
{
    public class HelpCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public HelpCommand(TelegramBotService telegramBotHelper)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }
        public override string Name => CommandNames.HelpCommand;

        public override async Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, StaticFields.HelpMsg, parseMode: ParseMode.MarkdownV2);
        }
    }
}
