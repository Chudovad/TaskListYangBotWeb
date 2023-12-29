namespace TaskListYangBotWeb.Data.Interfaces
{
    public interface IMessageRepository
    {
        bool Save();
        bool AddUserMessage(long userId, string textMsg);
    }
}
