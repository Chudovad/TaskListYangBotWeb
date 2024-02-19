using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class YangOnFavTaskEnvCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteTaskRepository _favoriteTaskRepository;
        private readonly IFavoriteEnvironmentRepository _favoriteEnvironmentRepository;

        public YangOnFavTaskEnvCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository, IFavoriteTaskRepository favoriteTaskRepository, IFavoriteEnvironmentRepository favoriteEnvironmentRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
            _favoriteTaskRepository = favoriteTaskRepository;
            _favoriteEnvironmentRepository = favoriteEnvironmentRepository;
        }
        public override string Name => CommandNames.YangOnFavTaskAndEnvCommand;

        public override async Task ExecuteAsync(Update update)
        {
            if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
            {
                string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);
                int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
                List<string?> listFavoriteTasks = _favoriteTaskRepository
                    .GetUserFavoriteTasks(update.Message.Chat.Id)
                    .Select(s => s.TaskName)
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                    .ToList();
                List<string?> listFavoriteEnvironments = _favoriteEnvironmentRepository
                    .GetUserFavoriteEnvironments(update.Message.Chat.Id)
                    .Select(s => s.EnvironmentName)
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                    .ToList();
                AutomaticTaskPickupService automaticTaskPickupService =
                    new AutomaticTaskPickupService(update.Message.Chat.Id, true, _telegramBotClient, tokenYang, typeSorting, listFavoriteTasks, CommandNames.YangOnFavTaskAndEnvCommand);
                automaticTaskPickupService.Start(listFavoriteEnvironments);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnFavTaskAndEnvCommand} уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
