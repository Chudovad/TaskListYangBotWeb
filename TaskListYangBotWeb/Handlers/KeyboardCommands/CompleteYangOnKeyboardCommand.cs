using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;

namespace TaskListYangBotWeb.Handlers.KeyboardCommands
{
    public class CompleteYangOnKeyboardCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public CompleteYangOnKeyboardCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }
        public override string Name => KeyboardCommandNames.CompleteYangOnCommandKeyboard;

        public async override Task ExecuteAsync(Update update)
        {
            CommandStatus.commandStatus[update.Message.Chat.Id] = false;
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда выключена", replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
