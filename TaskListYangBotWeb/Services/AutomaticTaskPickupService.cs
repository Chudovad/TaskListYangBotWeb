using System.Globalization;
using TaskListYangBotWeb.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Services
{
    public class AutomaticTaskPickupService
    {
        private const int waitTime = 2000;
        private const int periodOfSendingNotification = 30;

        public static async Task StartYangONCommand(long userId, bool withFavorite, TelegramBotClient _telegramBotClient, string tokenYang, int typeSorting, List<string> listFavorite, string commandName)
        {
            if (withFavorite && listFavorite.Count == 0)
            {
                await SendNoFavoriteTasksMessage(userId, _telegramBotClient);
                return;
            }
            else
            {
                await SendCommandStatusMessage(userId, _telegramBotClient, commandName);
            }

            CommandStatus.commandStatus[userId] = true;

            await StartTaskLoop(userId, withFavorite, tokenYang, listFavorite, typeSorting, _telegramBotClient, commandName);
        }

        private static async Task SendNoFavoriteTasksMessage(long chatId, TelegramBotClient _telegramBotClient)
        {
            string message = $"Для использования команды {CommandNames.YangOnFavoriteCommand} или {CommandNames.YangOnEnvironmentCommand}, " +
                $"добавьте любимые задания или окружения в команде {CommandNames.FavoriteTasksCommand}, {CommandNames.FavoriteEnvironmentsCommand} или {CommandNames.YangCommand}," +
                $" нажав на кнопку 'В любимые задания' или 'В любимые окружения'";
            await _telegramBotClient.SendTextMessageAsync(chatId, message);
        }

        private static async Task SendCommandStatusMessage(long chatId, TelegramBotClient _telegramBotClient, string commandName)
        {
            var message = $"Команда {commandName} включена";
            await _telegramBotClient.SendTextMessageAsync(chatId, message, replyMarkup: StaticFields.Keyboard);
        }

        private static async Task StartTaskLoop(long userId, bool withFavorite, string tokenYang, List<string> listFavorite, int typeSorting, TelegramBotClient _telegramBotClient, string commandName)
        {
            List<dynamic> taskList;
            for (int i = 1; ; i++)
            {
                taskList = GetTaskList(withFavorite, tokenYang, listFavorite, typeSorting);

                if (!CommandStatus.commandStatus[userId])
                {
                    break;
                }
                else if (DateTime.Now.Minute % periodOfSendingNotification == 0)
                {
                    await _telegramBotClient.SendTextMessageAsync(userId, "Заданий ещё нет");
                }
                else if (taskList.Count == 0)
                {
                    Thread.Sleep(waitTime);
                }
                else if (CommandStatus.commandStatus[userId])
                {
                    await TakeTask(userId, withFavorite, tokenYang, taskList, _telegramBotClient, commandName);
                }
            }
        }

        private static async Task TakeTask(long userId, bool withFavorite, string tokenYang, List<dynamic> taskList, TelegramBotClient _telegramBotClient, string commandName)
        {
            foreach (var item in taskList)
            {
                var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);

                if (takeTaskResponse.statusCode == 200)
                {
                    ParseYangService.GetMessageTakingTask(takeTaskResponse, _telegramBotClient, userId);
                    CommandStatus.commandStatus[userId] = false;
                    await _telegramBotClient.SendTextMessageAsync(userId, $"Команда {commandName} выключена", replyMarkup: new ReplyKeyboardRemove());
                    break;
                }
            }
            Thread.Sleep(waitTime);
        }

        private static List<dynamic> GetTaskList(bool withFavorite, string tokenYang, List<string> listFavorite, int typeSorting)
        {
            if (withFavorite)
                return ApplySorting(ParseYangService.RequestToApiTaskList(tokenYang)
                    .Where(x => listFavorite.Any(q =>
                        x.description != null && x.description.ToString().Contains(q) ||
                        x.title != null && x.title.ToString().Contains(q))), typeSorting);
            else
                return ApplySorting(ParseYangService.RequestToApiTaskList(tokenYang)
                    .Where(x => x.projectMetaInfo.ignored != true && x.pools[0].activeAssignments == null), typeSorting);
        }

        public static List<dynamic> ApplySorting(IEnumerable<dynamic> tasks, int typeSorting)
        {
            switch (typeSorting)
            {
                case 1:
                    return tasks.OrderBy(r => double.Parse((string)r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                case 0:
                    return tasks.OrderByDescending(r => double.Parse((string)r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
                default:
                    return tasks.ToList();
            }
        }
    }
}
