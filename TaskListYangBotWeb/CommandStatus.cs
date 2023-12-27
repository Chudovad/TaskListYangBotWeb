using System.Globalization;
using TaskListYangBotWeb.Services;

namespace TaskListYangBotWeb
{
    public class CommandStatus
    {
        public static Dictionary<long, List<dynamic>> taskListsUsers = new Dictionary<long, List<dynamic>>();
        public static Dictionary<long, bool> commandStatus = new Dictionary<long, bool>();

        public static void AddToDictionaryTaskList(int sortingType, string tokenYang, long chatId)
        {
            List<dynamic> sortedTasks;
            if (sortingType == 1)
            {
                sortedTasks = ParseYangService.RequestToApiTaskList(tokenYang)
                            .Where(x => x.projectMetaInfo.ignored != true)
                            .OrderBy(r => double.Parse((string)r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
            }
            else if (sortingType == 0)
            {
                sortedTasks = ParseYangService.RequestToApiTaskList(tokenYang)
                            .Where(x => x.projectMetaInfo.ignored != true)
                            .OrderByDescending(r => double.Parse((string)r.pools[0].reward, CultureInfo.InvariantCulture)).ToList();
            }
            else
            {
                sortedTasks = ParseYangService.RequestToApiTaskList(tokenYang)
                            .Where(x => x.projectMetaInfo.ignored != true)
                            .ToList();
            }

            if (!taskListsUsers.ContainsKey(chatId))
            {
                taskListsUsers.Add(chatId, sortedTasks);
            }
            else
            {
                taskListsUsers[chatId] = sortedTasks;
            }
        }
    }
}
