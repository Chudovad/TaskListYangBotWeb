using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers
{
    public abstract class BaseHandler
    {
        public abstract string Name { get; }

        public abstract Task ExecuteAsync(Update update);

    }
}
