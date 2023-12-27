using System.Globalization;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Handlers;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Services
{
    public class AutomaticTaskPickupService
    {
        public static async Task StartYangONCommand(Update update, bool withFavorite, TelegramBotClient _telegramBotClient, IUserRepository _userRepository, IFavoriteTaskRepository _favoriteTaskRepository)
        {

            string tokenYang = _userRepository.GetUserToken(update.Message.Chat.Id);
            int typeSorting = _userRepository.GetUserSorting(update.Message.Chat.Id);
            List<FavoriteTask> listFavoriteTasks = _favoriteTaskRepository.GetUserFavoriteTask(update.Message.Chat.Id).ToList();
            int waitTime = 2000;
            int periodOfSendingNotification = 500;

            if (withFavorite == true && listFavoriteTasks.Count == 0)
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, 
                    $"Для использования команды {CommandNames.YangOnFavoriteCommand} добавьте любимые задания в команде {CommandNames.FavoriteTasksCommand} или {CommandNames.YangCommand}, нажав на кнопку 'В любимые' в задании");
                return;
            }
            else if (withFavorite == true && listFavoriteTasks.Count != 0)
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnFavoriteCommand} включена", replyMarkup: StaticFields.Keyboard);
            else
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Команда {CommandNames.YangOnCommand} включена", replyMarkup: StaticFields.Keyboard);
            CommandStatus.commandStatus[update.Message.Chat.Id] = true;
            List<dynamic> taskList;
            for (int i = 1; ; i++)
            {
                taskList = GetTaskList(withFavorite, tokenYang, listFavoriteTasks, typeSorting);

                if (CommandStatus.commandStatus[update.Message.Chat.Id] == false)
                {
                    break;
                }
                else if (i % periodOfSendingNotification == 0)
                {
                    await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Заданий ещё нет");
                }
                else if (taskList.Count == 0)
                {
                    Thread.Sleep(waitTime);
                }
                else if (CommandStatus.commandStatus[update.Message.Chat.Id] == true)
                {
                    await TakeTask(update, withFavorite, tokenYang, waitTime, taskList, _telegramBotClient);
                }
            }

        }

        private static async Task TakeTask(Update update, bool withFavorite, string tokenYang, int waitTime, List<dynamic> taskList, TelegramBotClient _telegramBotClient)
        {
            int numTask = 0;

            foreach (var item in taskList)
            {
                numTask++;

                var takeTaskResponse = ParseYangService.RequestToApiTakeTask(item.pools[0].id.ToString(), tokenYang);

                if (takeTaskResponse.statusCode == 200)
                {
                    ParseYangService yangCommand = new ParseYangService();
                    string message = yangCommand.MessageTakeTask(takeTaskResponse);
                    if (!message.Contains("Ошибка"))
                    {
                        IReplyMarkup replyMarkup = CreateButtons.GetButton(takeTaskResponse);
                        await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, message, replyMarkup: replyMarkup);
                        CommandStatus.commandStatus[update.Message.Chat.Id] = false;
                        await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, withFavorite == true ? $"Команда {CommandNames.YangOnFavoriteCommand} выключена" : $"Команда {CommandNames.YangOnCommand} выключена", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    else
                        await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, message);
                }
            }
            Thread.Sleep(waitTime);
        }

        private static List<dynamic> GetTaskList(bool withFavorite, string tokenYang, List<FavoriteTask> listFavoriteTasks, int typeSorting)
        {
            if (withFavorite)
                return ApplySorting(ParseYangService.RequestToApiTaskList(tokenYang)
                    .Where(x => listFavoriteTasks.Any(q => x.description.Contains(q.TaskName) || x.title.Contains(q.TaskName))), typeSorting);
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
