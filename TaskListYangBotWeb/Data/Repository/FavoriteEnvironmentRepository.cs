using Microsoft.EntityFrameworkCore;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Repository
{
    public class FavoriteEnvironmentRepository : IFavoriteEnvironmentRepository
    {
        public readonly ApplicationContext _context;

        public FavoriteEnvironmentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool AddFavoriteEnvironment(long userId, string environmentName, long poolId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null && !CheckUserEnvironment(userId, environmentName))
            {
                FavoriteEnvironment favoriteEnvironment = new FavoriteEnvironment()
                {
                    EnvironmentName = environmentName,
                    PoolId = poolId,
                    User = user,
                };
                _context.Add(favoriteEnvironment);
                return Save();
            }
            return false;
        }

        public bool CheckUserEnvironment(long userId, string environmentName)
        {
            return _context.FavoriteEnvironments.Include(c => c.User).Any(x => x.User.UserId == userId && x.EnvironmentName == environmentName);
        }

        public bool DeleteFavoriteEnvironment(long userId, long poolId)
        {
            var userFavoriteEnvironment = _context.FavoriteEnvironments.Include(c => c.User).FirstOrDefault(u => u.User.UserId == userId && u.PoolId == poolId);
            if (userFavoriteEnvironment != null)
            {
                _context.Remove(userFavoriteEnvironment);
                return Save();
            }
            return false;
        }

        public ICollection<FavoriteEnvironment> GetUserFavoriteEnvironments(long userId)
        {
            return _context.FavoriteEnvironments.Include(c => c.User).Where(u => u.User.UserId == userId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
