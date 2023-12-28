using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class YangCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public YangCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.YangCommand;

        public override async Task ExecuteAsync(Update update)
        {
            PaginationService pagination = new PaginationService();
            int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
            string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);

            CommandStatus.AddToDictionaryTaskList(typeSorting, tokenYang, update.Message.Chat.Id);
            pagination.AddToDictionaryNumberPage(update.Message.Chat.Id, 0);
            if (CommandStatus.taskListsUsers[update.Message.Chat.Id].Count <= 20)
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Количество заданий: " + CommandStatus.taskListsUsers[update.Message.Chat.Id].Count + "\r\n" + "🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸\r\n");
                await CreateMsgTask(update, tokenYang, CommandStatus.taskListsUsers[update.Message.Chat.Id], _telegramBotClient);
            }
            else
            {
                List<dynamic> pageTasks = pagination.GetPage(CommandStatus.taskListsUsers[update.Message.Chat.Id], PaginationService.numberOfPageDic[update.Message.Chat.Id], 20);

                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Количество заданий: " + CommandStatus.taskListsUsers[update.Message.Chat.Id].Count + "\r\n" + "🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸\r\n",
                    replyMarkup: pageTasks.Count >= 20 ? StaticFields.KeyboardForYangCommand : new ReplyKeyboardRemove());
                await CreateMsgTask(update, tokenYang, pageTasks, _telegramBotClient);
            }
        }

        public static async Task CreateMsgTask(Update update, string tokenYang, List<dynamic> taskList, TelegramBotClient client)
        {
            if (taskList.Count == 0)
            {
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Заданий нет", replyMarkup: new ReplyKeyboardRemove());
                PaginationService.numberOfPageDic[update.Message.Chat.Id] = 0;
            }
            else
            {
                foreach (var item in taskList)
                {
                    if (item.pools[0].activeAssignments != null)
                    {
                        var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);
                        ParseYangService.GetMessageTakingTask(takeTaskResponse, client, update);
                    }
                    else
                    {
                        string message = "🔹" + "Задание 🔹\r\n" + item.description + "(" + item.pools[0].reward + ")" + "\r\n" + item.title + "\r\n";
                        IReplyMarkup replyMarkup = CreateButtons.GetButton((int)item.pools[0].id, "Приступить", "В любимые");
                        await client.SendTextMessageAsync(update.Message.Chat.Id, message, replyMarkup: replyMarkup);
                        Thread.Sleep(100);
                    }

                }
            }
        }
    }
}
