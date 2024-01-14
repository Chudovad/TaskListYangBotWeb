using Microsoft.EntityFrameworkCore;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Repository
{
    public class RoleRepository : IRoleRepository
    {
        public readonly ApplicationContext _context;

        public RoleRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Role GetRole(int id)
        {
            return _context.Roles.FirstOrDefault(r => r.Id == id);
        }

        public Role GetRole(string roleName)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleName == roleName);
        }
        
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
