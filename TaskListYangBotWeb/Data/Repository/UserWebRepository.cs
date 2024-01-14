using Microsoft.EntityFrameworkCore;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb.Services;

namespace TaskListYangBotWeb.Data.Repository
{
    public class UserWebRepository : IUserWebRepository
    {
        public readonly ApplicationContext _context;

        public UserWebRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool CreateUser(UserWebRegister user, int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            if (!_context.UsersWeb.Any(u => u.Username == user.Username) && role != null)
            {
                var newUser = new UserWeb()
                {
                    Username = user.Username,
                    PasswordHash = EncryptionService.EncryptStringToBytes(user.Password, StaticFields.passwordEncryption),
                    Role = role,
                };
                _context.Add(newUser);
                return Save();
            }
            return false;
        }

        public UserWeb AuthenticateUser(UserWebLogin userWeb)
        {
            return _context.UsersWeb.Include(x => x.Role).FirstOrDefault(
                    u => u.Username.ToLower() == userWeb.Username.ToLower() 
                    && u.PasswordHash.SequenceEqual(EncryptionService.EncryptStringToBytes(userWeb.Password, StaticFields.passwordEncryption)));
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public UserWeb GetUser(string username)
        {
            return _context.UsersWeb.FirstOrDefault(u => u.Username == username);
        }
    }
}
