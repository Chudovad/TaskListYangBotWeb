using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data.Repository
{
    public class MessageRepository : IMessageRepository
    {
        public readonly ApplicationContext _context;

        public MessageRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool AddUserMessage(long userId, string textMsg)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                Message newMessage = new Message
                {
                    TextMessage = textMsg,
                    DateTime = DateTime.Now.AddHours(StaticFields.NumberOfAddedHoursForServer),
                    User = user
                };
                _context.Add(newMessage);
                return Save();
            }
            return false;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
