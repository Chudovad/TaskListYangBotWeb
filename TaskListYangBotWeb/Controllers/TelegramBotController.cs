using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Services.Interfaces;

namespace TaskListYangBotWeb.Controllers
{
    [Route("api/message/update")]
    [ApiController]
    public class TelegramBotController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public TelegramBotController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] object _update)
        {
            var update = JsonConvert.DeserializeObject<Update>(_update.ToString());

            try
            {
                await _commandExecutor.Execute(update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok();
            }
            return Ok();
        }
    }
}
