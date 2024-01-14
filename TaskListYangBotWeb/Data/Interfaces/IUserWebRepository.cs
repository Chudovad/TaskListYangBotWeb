using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IUserWebRepository
    {
        bool Save();
        bool CreateUser(UserWebRegister user, int roleId);
        UserWeb AuthenticateUser(UserWebLogin userWeb);
        UserWeb GetUser(string username);
    }
}
