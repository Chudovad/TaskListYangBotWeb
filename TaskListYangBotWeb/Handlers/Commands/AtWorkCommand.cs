using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            ParseYangService yangCommand = new ParseYangService();
            string userToken = _userRepository.GetUserToken(update.Message.Chat.Id);
            List<dynamic> taskListInProgress = ParseYangService.RequestToApiTaskList(userToken).Where(x => x.pools[0].activeAssignments != null && x.pools[0].activeAssignments.Count > 0).ToList();
            if (taskListInProgress.Count != 0)
            {
                foreach (var item in taskListInProgress)
                {
                    var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), userToken);
                    string message = yangCommand.MessageTakeTask(takeTaskResponse);
                    if (!message.Contains("Ошибка"))
                    {
                        IReplyMarkup replyMarkup = CreateButtons.GetButton(takeTaskResponse);
                        await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, message, replyMarkup: replyMarkup);
                    }
                    else
                        await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, message);
                }
            }
            else
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Заданий в работе нет");
        }
    }
}
