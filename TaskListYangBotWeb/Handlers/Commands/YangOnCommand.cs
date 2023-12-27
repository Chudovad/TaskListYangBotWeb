using System.Globalization;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class YangOnCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public YangOnCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CommandNames.YangOnCommand;

        public override async Task ExecuteAsync(Update update)
        {
            if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
            {
                await AutomaticTaskPickupService.StartYangONCommand(update, false, _telegramBotClient, _userRepository, _favoriteTaskRepository);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда /yangon или /yangonfavorite уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
