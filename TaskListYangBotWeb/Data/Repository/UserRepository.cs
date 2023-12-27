using Telegram.Bot.Types;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;
using TaskListYangBotWeb;

namespace TaskListYangBotWeb.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool CreateUser(Update update)
        {
            if (!_context.Users.Any(u => u.UserId == update.Message.Chat.Id))
            {
                var user = new Models.User()
                {
                    UserId = update.Message.Chat.Id,
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName,
                    UserName = update.Message.Chat.Username,
                    DateReg = DateTime.Now.AddHours(StaticFields.NumberOfAddedHoursForServer),
                    Token = Encryption.EncryptStringToBytes("", StaticFields.passwordEncryption),
                    TypeSorting = 2
                };
                _context.Add(user);
                return Save(); 
            }
            return false;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public Models.User GetUser(long userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public string GetUserToken(long userId)
        {
            var user = GetUser(userId);

            if (user == null)
                return "";
            return Encryption.DecryptStringFromBytes(user.Token, StaticFields.passwordEncryption).Replace("\0", "");
        }

        public bool UpdateUserToken(long userId, string token)
        {
            var user = _context.Users.FirstOrDefault(u =>  userId == u.UserId);
            if (user != null)
            {
                user.Token = Encryption.EncryptStringToBytes(token, StaticFields.passwordEncryption);
                _context.Users.Update(user);
                return Save(); 
            }
            return false;
        }

        public int GetUserSorting(long userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId).TypeSorting;
        }

        public bool UpdateUserSorting(long userId, int typeSorting)
        {
            var user = _context.Users.FirstOrDefault(u => userId == u.UserId);
            if(user != null)
            {
                user.TypeSorting = typeSorting;
                return Save();
            }
            return false;
        }
    }
}
