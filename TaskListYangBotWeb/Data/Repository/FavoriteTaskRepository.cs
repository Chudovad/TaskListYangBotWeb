using Microsoft.EntityFrameworkCore;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Repository
{
    public class FavoriteTaskRepository : IFavoriteTaskRepository
    {
        public readonly ApplicationContext _context;

        public FavoriteTaskRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool AddFavoriteTask(long userId, string taskName, long poolId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null && !CheckUserTask(userId, taskName))
            {
                FavoriteTask favoriteTask = new FavoriteTask()
                {
                    TaskName = taskName,
                    PoolId = poolId,
                    User = user,
                };
                _context.Add(favoriteTask);
                return Save();
            }
            return false;
        }

        public bool CheckUserTask(long userId, string taskName)
        {
            return _context.FavoriteTasks.Include(c => c.User).Any(x => x.User.UserId == userId && x.TaskName == taskName);
        }

        public bool DeleteFavoriteTask(long userId, long poolId)
        {
            var userFavoriteTask = _context.FavoriteTasks.Include(c => c.User).FirstOrDefault(u => u.User.UserId == userId && u.PoolId == poolId);
            _context.Remove(userFavoriteTask);
            return Save();
        }

        public ICollection<FavoriteTask> GetUserFavoriteTasks(long userId)
        {
            return _context.FavoriteTasks.Include(c => c.User).Where(u => u.User.UserId == userId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
