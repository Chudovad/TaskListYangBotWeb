using System.Globalization;
using TaskListYangBotWeb.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Services
{
    public class AutomaticTaskPickupService
    {
        private Task _task;
        private CancellationTokenSource _cts;
        private readonly TimeSpan notificationInterval = TimeSpan.FromMinutes(30);
        private const int waitTime = 2000;

        public bool IsRunning => _task != null && !_task.IsCompleted;

        public void Start(long userId, bool withFavorite, TelegramBotClient _telegramBotClient, string tokenYang, int typeSorting, List<string> listFavorite, string commandName)
        {
            _cts = new CancellationTokenSource();
            _task = Task.Run(async () => await StartYangONCommand(_cts.Token, userId, withFavorite, _telegramBotClient, tokenYang, typeSorting, listFavorite, commandName), _cts.Token);
        }

        public async Task StopAsync()
        {
            if (!IsRunning) return;

            _cts.Cancel();

            await Task.WhenAny(_task, Task.Delay(Timeout.Infinite));

            _cts.Dispose();
        }

        public async Task StartYangONCommand(CancellationToken cancellationToken, long userId, bool withFavorite, TelegramBotClient _telegramBotClient, string tokenYang, int typeSorting, List<string> listFavorite, string commandName)
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

            await StartTaskLoop(cancellationToken, userId, withFavorite, tokenYang, listFavorite, typeSorting, _telegramBotClient, commandName);
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

        private async Task StartTaskLoop(CancellationToken cancellationToken, long userId, bool withFavorite, string tokenYang, List<string> listFavorite, int typeSorting, TelegramBotClient _telegramBotClient, string commandName)
        {
            List<dynamic> taskList;
            DateTime lastNotificationTime = DateTime.Now;
            while (!cancellationToken.IsCancellationRequested)
            {
                taskList = GetTaskList(withFavorite, tokenYang, listFavorite, typeSorting);

                if (!CommandStatus.commandStatus[userId])
                {
                    await StopAsync();
                }
                else if (DateTime.Now - lastNotificationTime >= notificationInterval)
                {
                    lastNotificationTime = DateTime.Now;
                    await _telegramBotClient.SendTextMessageAsync(userId, "Заданий ещё нет");
                }
                else if (taskList.Count == 0)
                {
                    Thread.Sleep(waitTime);
                }
                else if (CommandStatus.commandStatus[userId])
                {
                    await TakeTask(userId, tokenYang, taskList, _telegramBotClient, commandName);
                }
            }
        }

        private async Task TakeTask(long userId, string tokenYang, List<dynamic> taskList, TelegramBotClient _telegramBotClient, string commandName)
        {
            foreach (var item in taskList)
            {
                var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);

                if (takeTaskResponse.statusCode == 200)
                {
                    ParseYangService.GetMessageTakingTask(takeTaskResponse, _telegramBotClient, userId);
                    CommandStatus.commandStatus[userId] = false;
                    await _telegramBotClient.SendTextMessageAsync(userId, $"Команда {commandName} выключена", replyMarkup: new ReplyKeyboardRemove());
                    await StopAsync();
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
