using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IRoleRepository
    {
        bool Save();
        Role GetRole(int id);
        Role GetRole(string roleName);
    }
}
