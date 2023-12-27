using System.Collections;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IFavoriteTaskRepository
    {
        bool Save();
        bool AddFavoriteTask(long userId, List<dynamic> userTasks);
        ICollection<FavoriteTask> GetUserFavoriteTask(long userId);
    }
}
