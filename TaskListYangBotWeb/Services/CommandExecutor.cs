using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Handlers;
using TaskListYangBotWeb.Services.Interfaces;

namespace TaskListYangBotWeb.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly List<BaseHandler> _commands;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public CommandExecutor(IServiceProvider serviceProvider, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _commands = serviceProvider.GetServices<BaseHandler>().ToList();
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                if (update.Message.Text == null)
                    return;
                _messageRepository.AddUserMessage(update.Message.Chat.Id, update.Message.Text);
                if (update.Message.Text != CommandNames.StartCommand 
                    && ParseYangService.RequestToApiCheckToken(_userRepository.GetUserToken(update.Message.Chat.Id)).message != null 
                    && update.Message.ReplyToMessage == null)
                {
                    await _commands.FirstOrDefault(c => c.Name == CommandNames.StartCommand).ExecuteAsync(update);
                    return;
                }
                if (Uri.IsWellFormedUriString(update.Message.Text, UriKind.Absolute))
                {
                    await _commands.FirstOrDefault(c => c.Name == CommandNames.CreateLinkCommand).ExecuteAsync(update);
                    return;
                }
                if (_commands.Any(c => update.Message.Text == c.Name))
                {
                    if (!CommandStatus.commandStatus.ContainsKey(update.Message.Chat.Id))
                        CommandStatus.commandStatus.Add(update.Message.Chat.Id, false);
                    await ExecuteCommand(update.Message.Text, update);
                    return;
                }
                if (update.Message.ReplyToMessage != null && update.Message.ReplyToMessage.Text != null && _commands.Any(c => update.Message.ReplyToMessage.Text.Contains(c.Name)))
                {
                    await ExecuteReplay(update.Message.ReplyToMessage.Text, update);
                    return;
                }
                await _commands.FirstOrDefault(c => c.Name == CommandNames.DefaultCommand).ExecuteAsync(update);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                _messageRepository.AddUserMessage(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Data);
                if (update?.CallbackQuery != null && _commands.Any(c => update.CallbackQuery.Data.Contains(c.Name)))
                {
                    await ExecuteCallback(update.CallbackQuery.Data, update);
                    return;
                }
            }
        }
        private async Task ExecuteCommand(string commandName, Update update)
        {
            BaseHandler _lastCommand = _commands.First(x => x.Name == commandName);
            await _lastCommand.ExecuteAsync(update);
        }
        private async Task ExecuteReplay(string replyString, Update update)
        {
            BaseHandler _lastCommand = _commands.First(x => replyString.StartsWith(x.Name));
            await _lastCommand.ExecuteAsync(update);
        }
        private async Task ExecuteCallback(string callback, Update update)
        {
            BaseHandler _lastCommand = _commands.First(x => callback.Contains(x.Name));
            await _lastCommand.ExecuteAsync(update);
        }
    }
}
