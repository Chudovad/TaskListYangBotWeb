using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IUserRepository
    {
        bool CreateUser(Update update);
        bool Save();
        Models.User GetUser(long userId);
        string GetUserToken(long userId);
        bool UpdateUserToken(long userId, string token);
        int GetUserSorting(long userId);
        bool UpdateUserSorting(long userId, int typeSorting);

    }
}
