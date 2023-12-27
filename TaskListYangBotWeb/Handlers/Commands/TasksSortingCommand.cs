using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class TasksSortingCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public TasksSortingCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.TasksSortingCommand;

        public override async Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                StaticFields.GetTaskSortingText(_userRepository.GetUserSorting(update.Message.Chat.Id)),
                replyMarkup: CreateButtons.GetButton(StaticFields.TypesSorting));
        }
    }
}
