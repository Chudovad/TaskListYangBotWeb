using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaskListYangBotWeb.Handlers.Commands
{
    internal class NormaCommand : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public NormaCommand(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CommandNames.NormaCommand;

        public override async Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, CreateMsgNorma(_userRepository.GetUserToken(update.Message.Chat.Id)));
        }

        private string CreateMsgNorma(string tokenYang)
        {
            string completedEmoji = "🟩";
            string notCompletedEmoji = "⬜️";
            string progressBar;
            string message;
            List<dynamic> norm = ParseYangService.RequestToApiCheckNormValue(tokenYang);
            if (norm != null)
            {
                int countCompletedEmoji = norm[1].currentNormValue < norm[1].destinationNormValue ? (int)norm[1].currentNormValue * 10 / (int)norm[1].destinationNormValue : 10;
                double countPercent = norm[1].destinationNormValue != 0 ? norm[1].currentNormValue * 100 / norm[1].destinationNormValue : 0;
                progressBar = string.Concat(Enumerable.Repeat(completedEmoji, countCompletedEmoji)) + string.Concat(Enumerable.Repeat(notCompletedEmoji, 10 - countCompletedEmoji));
                double remains = norm[1].currentNormValue < norm[1].destinationNormValue ? norm[1].destinationNormValue - norm[1].currentNormValue : 0;
                message = $"Выполнено - {norm[1].currentNormValue} БО\nЦель - {norm[1].destinationNormValue} БО\nОсталось - {Math.Round(remains, 3)} БО\n\n{Math.Round(countPercent, 3)}%\n{progressBar}";
            }
            else
            {
                message = "Нет нормы";
            }
            return message;
        }
    }
}
