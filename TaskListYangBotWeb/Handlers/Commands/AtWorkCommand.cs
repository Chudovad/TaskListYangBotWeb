using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class AtWorkCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public AtWorkCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.AtWorkCommand;

        public override async Task ExecuteAsync(Update update)
        {
            string userToken = _userRepository.GetUserToken(update.Message.Chat.Id);
            List<dynamic> taskListInProgress = ParseYangService.RequestToApiTaskList(userToken).Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            if (taskListInProgress.Count != 0)
            {
                foreach (var item in taskListInProgress)
                {
                    var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), userToken);
                    ParseYangService.GetMessageTakingTask(takeTaskResponse, _telegramBotClient, update.Message.Chat.Id);
                }
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Заданий в работе нет");
        }
    }
}
