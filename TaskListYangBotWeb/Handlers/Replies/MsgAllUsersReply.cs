using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Handlers.Callbacks;
using TaskListYangBotWeb.Helper;

namespace TaskListYangBotWeb.Handlers.Replies
{
    public class MsgAllUsersReply : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public MsgAllUsersReply(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => ReplayNames.MsgAllUsersReply;

        public async override Task ExecuteAsync(Update update)
        {

        }
    }
}
