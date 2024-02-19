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
        private readonly long userId;
        private readonly bool withFavorite;
        private readonly TelegramBotClient _telegramBotClient;
        private readonly string tokenYang;
        private readonly int typeSorting;
        private readonly List<string> listFavoriteTask;
        private readonly string commandName;

        public bool IsRunning => _task != null && !_task.IsCompleted;

        public AutomaticTaskPickupService(long userId, bool withFavorite, TelegramBotClient _telegramBotClient, string tokenYang, int typeSorting, List<string> listFavoriteTask, string commandName)
        {
            this.userId = userId;
            this.withFavorite = withFavorite;
            this._telegramBotClient = _telegramBotClient;
            this.tokenYang = tokenYang;
            this.typeSorting = typeSorting;
            this.listFavoriteTask = listFavoriteTask;
            this.commandName = commandName;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _task = Task.Run(async () => await StartYangONCommand(_cts.Token), _cts.Token);
        }

        public void Start(List<string> listFavoriteEnv)
        {
            _cts = new CancellationTokenSource();
            _task = Task.Run(async () => await StartYangOnFavTaskAndEnv(_cts.Token, listFavoriteEnv), _cts.Token);
        }

        private async Task StartYangOnFavTaskAndEnv(CancellationToken cancellationToken, List<string> listFavoriteEnv)
        {
            if (listFavoriteEnv.Count == 0 && listFavoriteTask.Count == 0)
            {
                await SendNoFavoriteTasksMessage();
                return;
            }
            else
            {
                await SendCommandStatusMessage();
            }
            CommandStatus.commandStatus[userId] = true;

            await StartTaskLoop(cancellationToken, listFavoriteEnv);
        }

        public async Task StopAsync()
        {
            if (!IsRunning) return;

            _cts.Cancel();

            await Task.WhenAny(_task, Task.Delay(Timeout.Infinite));

            _cts.Dispose();
        }

        private async Task StartYangONCommand(CancellationToken cancellationToken)
        {
            if (withFavorite && listFavoriteTask.Count == 0)
            {
                await SendNoFavoriteTasksMessage();
                return;
            }
            else
            {
                await SendCommandStatusMessage();
            }

            CommandStatus.commandStatus[userId] = true;

            await StartTaskLoop(cancellationToken);
        }

        private async Task SendNoFavoriteTasksMessage()
        {
            string message = $"Для использования команд {CommandNames.YangOnFavoriteCommand}, {CommandNames.YangOnEnvironmentCommand} или {CommandNames.YangOnFavTaskAndEnvCommand}, " +
                $"добавьте любимые задания или окружения в команде {CommandNames.FavoriteTasksCommand}, {CommandNames.FavoriteEnvironmentsCommand} или {CommandNames.YangCommand}," +
                $" нажав на кнопку 'В любимые задания' или 'В любимые окружения'";
            await _telegramBotClient.SendTextMessageAsync(userId, message);
        }

        private async Task SendCommandStatusMessage()
        {
            var message = $"Команда {commandName} включена";
            await _telegramBotClient.SendTextMessageAsync(userId, message, replyMarkup: StaticFields.Keyboard);
        }

        private async Task StartTaskLoop(CancellationToken cancellationToken)
        {
            List<dynamic> taskList;
            DateTime lastNotificationTime = DateTime.Now;
            while (!cancellationToken.IsCancellationRequested)
            {
                taskList = GetTaskList(withFavorite, tokenYang, listFavoriteTask, typeSorting);

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
                    await TakeTask(taskList);
                }
            }
        }

        private async Task StartTaskLoop(CancellationToken cancellationToken, List<string> listFavoriteEnv)
        {
            List<dynamic> taskList;
            DateTime lastNotificationTime = DateTime.Now;
            while (!cancellationToken.IsCancellationRequested)
            {
                taskList = ApplySorting(ParseYangService.RequestToApiTaskList(tokenYang)
                    .Where(x => listFavoriteTask.Any(q =>
                        x.description != null && x.description.ToString().Contains(q) ||
                        x.title != null && x.title.ToString().Contains(q)) &&
                        listFavoriteEnv.Any(q =>
                        x.description != null && x.description.ToString().Contains(q) ||
                        x.title != null && x.title.ToString().Contains(q))), typeSorting);

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
                    await TakeTask(taskList);
                }
            }
        }

        private async Task TakeTask(List<dynamic> taskList)
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
