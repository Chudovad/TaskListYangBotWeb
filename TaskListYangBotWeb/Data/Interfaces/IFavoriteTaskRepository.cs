using System.Collections;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IFavoriteTaskRepository
    {
        bool Save();
        bool AddFavoriteTask(long userId, string taskName, long poolId);
        ICollection<FavoriteTask> GetUserFavoriteTasks(long userId);
        bool DeleteFavoriteTask(long userId, long poolId);
        bool CheckUserTask(long userId, string taskName);
    }
}
