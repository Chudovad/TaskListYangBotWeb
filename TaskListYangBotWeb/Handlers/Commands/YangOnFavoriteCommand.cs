using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class YangOnFavoriteCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;

        public YangOnFavoriteCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository, IFavoriteTaskRepository favoriteTaskRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
            _favoriteTaskRepository = favoriteTaskRepository;
        }
        public override string Name => CommandNames.YangOnFavoriteCommand;

        public override async Task ExecuteAsync(Update update)
        {

            if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
            {
                await AutomaticTaskPickupService.StartYangONCommand(update, true, _telegramBotClient, _userRepository, _favoriteTaskRepository);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда /yangon или /yangonfavorite уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
