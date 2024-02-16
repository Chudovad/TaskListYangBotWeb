using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class YangOnEnvironmentCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public YangOnEnvironmentCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }
        public override string Name => CommandNames.YangOnEnvironmentCommand;

        public override async Task ExecuteAsync(Update update)
        {
            if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
            {
                string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);
                int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
                List<string?> listFavoriteTasks = _favoriteEnvironmentRepository
                    .GetUserFavoriteEnvironments(update.Message.Chat.Id)
                    .Select(s => s.EnvironmentName)
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                    .ToList();
                await AutomaticTaskPickupService.StartYangONCommand(update.Message.Chat.Id, true, _telegramBotClient, tokenYang, typeSorting, listFavoriteTasks, CommandNames.YangOnEnvironmentCommand);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnEnvironmentCommand} уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
