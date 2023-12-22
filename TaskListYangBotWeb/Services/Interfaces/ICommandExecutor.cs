using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Services.Interfaces
{
    public interface ICommandExecutor
    {
        Task Execute(Update update);
    }
}
