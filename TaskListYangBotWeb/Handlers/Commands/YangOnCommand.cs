using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

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
                string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);
                int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
                List<string?> listFavoriteTasks = _favoriteTaskRepository
                    .GetUserFavoriteTasks(update.Message.Chat.Id)
                    .Select(s => s.TaskName)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                await AutomaticTaskPickupService.StartYangONCommand(update.Message.Chat.Id, false, _telegramBotClient, tokenYang, typeSorting, listFavoriteTasks, CommandNames.YangOnCommand);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnCommand} уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
