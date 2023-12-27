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

        public bool AddFavoriteTask(long userId, List<dynamic> userTasks)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                FavoriteTask favoriteTask = new FavoriteTask()
                {
                    TaskName = userTasks[0].description,
                    PoolId = userTasks[0].pools[0].id,
                    User = user,
                };
                _context.Add(favoriteTask);
                return Save();
            }
            return false;
        }

        public ICollection<FavoriteTask> GetUserFavoriteTask(long userId)
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
