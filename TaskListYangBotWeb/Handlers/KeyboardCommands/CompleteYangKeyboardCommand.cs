using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;

namespace TaskListYangBotWeb.Handlers.KeyboardCommands
{
    public class CompleteYangKeyboardCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;

        public CompleteYangKeyboardCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
        }
        public override string Name => KeyboardCommandNames.CompleteYangCommandKeyboard;

        public async override Task ExecuteAsync(Update update)
        {
            PaginationService.numberOfPageDic[update.Message.Chat.Id] = 0;
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда выключена", replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
