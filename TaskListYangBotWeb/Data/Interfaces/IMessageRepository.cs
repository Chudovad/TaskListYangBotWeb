namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IMessageRepository
    {
        bool Save();
        bool AddUserMessage(long userId, string textMsg);
        ICollection<Models.Message> GetUserMessages(int userId);

        ICollection<Models.Message> GetMessages();
    }
}
