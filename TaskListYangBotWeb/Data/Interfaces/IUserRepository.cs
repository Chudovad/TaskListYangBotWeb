using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IUserRepository
    {
        bool CreateUser(Update update);
        bool Save();
        Models.User GetUser(long userId);
        string GetUserToken(long userId);
    }
}
