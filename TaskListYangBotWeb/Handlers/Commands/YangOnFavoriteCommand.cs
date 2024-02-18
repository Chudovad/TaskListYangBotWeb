using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
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
                AutomaticTaskPickupService automaticTaskPickupService = new AutomaticTaskPickupService();
                string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);
                int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
                List<string?> listFavoriteTasks = _favoriteTaskRepository
                    .GetUserFavoriteTasks(update.Message.Chat.Id)
                    .Select(s => s.TaskName)
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                    .ToList();
                automaticTaskPickupService.Start(update.Message.Chat.Id, true, _telegramBotClient, tokenYang, typeSorting, listFavoriteTasks, CommandNames.YangOnFavoriteCommand);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnFavoriteCommand} уже запущена", replyMarkup: StaticFields.Keyboard);
            }
        }
    }
}
