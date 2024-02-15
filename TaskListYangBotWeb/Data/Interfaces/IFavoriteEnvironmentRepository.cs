using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IFavoriteEnvironmentRepository
    {
        bool Save();
        bool AddFavoriteEnvironment(long userId, string EnvironmentName, long poolId);
        ICollection<FavoriteEnvironment> GetUserFavoriteEnvironments(long userId);
        bool DeleteFavoriteEnvironment(long userId, long poolId);
        bool CheckUserEnvironment(long userId, string EnvironmentName);
    }
}
