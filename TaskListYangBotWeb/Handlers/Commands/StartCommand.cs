using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using TaskListYangTgBot;
using Telegram.Bot.Types.Enums;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class StartCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public StartCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Update update)
        {
            _userRepository.CreateUser(update);
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Привет\\!" + StaticFields.HelpMsg, parseMode: ParseMode.MarkdownV2);
        }
    }
}
