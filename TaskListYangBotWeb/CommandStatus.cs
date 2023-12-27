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
            List<dynamic> sortedTasks = AutomaticTaskPickupService.ApplySorting(ParseYangService.RequestToApiTaskList(tokenYang).Where(x => x.projectMetaInfo.ignored != true), sortingType);

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
