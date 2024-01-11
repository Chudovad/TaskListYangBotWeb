using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace TaskListYangBotWeb.Helper
{
    public class TelegramBotService
    {
        private readonly IConfiguration _configuration;
        private TelegramBotClient _botClient;

        public TelegramBotService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TelegramBotClient> GetBot()
        {
            if (_botClient != null)
            {
                return _botClient;
            }
            _botClient = new TelegramBotClient(_configuration["TokenTest"]);
            var webHook = $"{_configuration["UrlWebhook"]}api/message/update";
            await _botClient.SetWebhookAsync(webHook);
            return _botClient;
        }
    }
}
