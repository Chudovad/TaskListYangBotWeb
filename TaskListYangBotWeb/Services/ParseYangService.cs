using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Microsoft.Extensions.ObjectPool;
using Telegram.Bot.Types;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Services
{
    class ParseYangService
    {
        public static List<dynamic> RequestToApiTaskList(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(StaticFields.urlTaskList).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            List<dynamic> taskRespons = JsonConvert.DeserializeObject<List<dynamic>>(json);

            return taskRespons;
        }
        public static dynamic RequestToApiTaskTitle(string tokenYang, dynamic poolId)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(StaticFields.urlTaskTitle + poolId + "?userLangs=RU").Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            dynamic taskTitle = JsonConvert.DeserializeObject<dynamic>(json);

            return taskTitle;
        }
        public static dynamic RequestToApiCheckToken(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(StaticFields.urlCheckToken).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            dynamic respons = JsonConvert.DeserializeObject<dynamic>(json);

            return respons;
        }
        public static List<dynamic> RequestToApiCheckNormValue(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(StaticFields.urlCheckNorm).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            List<dynamic> respons = JsonConvert.DeserializeObject<List<dynamic>>(json);

            return respons;
        }
        public static dynamic RequestToApiTakeTask(string poolId, string tokenYang)
        {
            var body = CreateBodyRequest(poolId, tokenYang);
            string result;
            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, MaxDepth = 128 };

                var postJson = JsonConvert.SerializeObject(body);
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var payload = new StringContent(postJson, Encoding.UTF8, "application/json");
                var status = (int)client.PostAsync(StaticFields.urlTakeTask, payload).Result.StatusCode;

                result = client.PostAsync(StaticFields.urlTakeTask, payload).Result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result, settings);
                response.statusCode = status;
                response.title = RequestToApiTaskTitle(tokenYang, response.poolId).title;
                return response;
            }
        }

        public static void RequestToApiLeaveTask(string taskId, string tokenYang)
        {
            string urlLeaveTaskFull = StaticFields.urlLeaveTask + taskId + "/expire";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);

                dynamic body = new
                {
                    assignmentIssuingType = ""
                };
                var postJson = JsonConvert.SerializeObject(body);
                var payload = new StringContent(postJson, Encoding.UTF8, "application/json");
                var result = client.PostAsync(urlLeaveTaskFull, payload).Result.Content.ReadAsStringAsync().Result;
            }
        }

        private static dynamic CreateBodyRequest(string poolId, string tokenYang)
        {
            string refUuid = "";
            var taskList = RequestToApiTaskList(tokenYang);
            var _visibleGroupsMeta = new List<dynamic>();
            var _visibleGroupsUuids = new List<dynamic>();
            var _activeFilters = new List<dynamic>
            {
                new { name = "withTraining" },
                new { name = "withPostAccept" },
                new { name = "withAdult" },
                new { name = "withUnavailable" },
                new { name = "withIgnored" },
                new { name = "toHideUnavailableByDefault" }
            };

            var _activeSort = new List<dynamic>
            {
                new { field = "price", direction = "DESC" }
            };

            foreach (var taskListItem in taskList)
            {
                _visibleGroupsMeta.Add(new { uuid = taskListItem.groupUuid });
                _visibleGroupsUuids.Add(taskListItem.groupUuid);
                if (taskListItem.pools[0].id == Convert.ToInt32(poolId))
                {
                    refUuid = taskListItem.refUuid;
                }
            }

            var poolSelectionContext = new
            {
                visibleGroupsMeta = _visibleGroupsMeta,
                visibleGroupsUuids = _visibleGroupsUuids,
                activeFilters = _activeFilters,
                activeSorts = _activeSort
            };

            var body = new
            {
                poolId = Convert.ToInt32(poolId),
                refUuid = refUuid,
                poolSelectionContext = poolSelectionContext
            };
            return body;
        }

        public static async Task GetMessageTakingTask(dynamic takeTaskResponse, TelegramBotClient _telegramBotClient, Update update)
        {
            long chatId;
            if (update.Message != null)
                chatId = update.Message.Chat.Id;
            else
                chatId = update.CallbackQuery.Message.Chat.Id;
            try
            {
                string message;
                if (takeTaskResponse.statusCode == 200)
                {
                    if (takeTaskResponse.tasks != null)
                    {
                        string reward = takeTaskResponse.reward;
                        string projectName = takeTaskResponse.tasks[0].input_values.data == null ? takeTaskResponse.title : takeTaskResponse.tasks[0].input_values.data.version_info.project_name;
                        string environmentShort = "";
                        string environment = "";
                        string checkEnvironmentOld = "";

                        if (takeTaskResponse.tasks[0].input_values.data != null)
                        {
                            environmentShort = takeTaskResponse.tasks[0].input_values.data.testrun_info.environment != null ? $"({Regex.Replace(Convert.ToString(takeTaskResponse.tasks[0].input_values.data.testrun_info.environment), @"<[^>]+>|&nbsp;|&emsp;", " ")})" : "";
                            checkEnvironmentOld = takeTaskResponse.tasks[0].input_values.data.testrun_info.final_requester_code != null ? $"Код проверки окружения: {takeTaskResponse.tasks[0].input_values.data.testrun_info.final_requester_code}" : "";

                            if (takeTaskResponse.tasks[0].input_values.data.testrun_info.env_descr != null)
                            {
                                environment = $"Окружение: {Regex.Replace(Convert.ToString(takeTaskResponse.tasks[0].input_values.data.testrun_info.env_descr).Replace("unknown", ""), @"<[^>]+>|&nbsp;|&emsp;", " ")}";
                            }
                            else
                            {
                                environment = ParseWebEnvironment(takeTaskResponse);
                            }
                        }
                        IReplyMarkup replyMarkup = CreateButtons.GetButton(takeTaskResponse);
                        await _telegramBotClient.SendTextMessageAsync(chatId, $"🔹 Взято задание 🔹\r\n{projectName} ({reward})\r\n\r\n{environment}{environmentShort}\r\n{checkEnvironmentOld}", replyMarkup: replyMarkup);
                        return;
                    }
                }
                message = HandleErrorMessages(takeTaskResponse);
                await _telegramBotClient.SendTextMessageAsync(chatId, message);
            }
            catch (Exception ex)
            {
                await _telegramBotClient.SendTextMessageAsync(chatId, $"Ошибка! {ex.Message}");
            }
        }

        private static string HandleErrorMessages(dynamic takeTaskResponse)
        {
            if (takeTaskResponse.message == "There are no more assignments in current pool" || takeTaskResponse.message == "There are no more assignments in merged pools")
            {
                return "Ошибка! Задание забрали";
            }
            else if (takeTaskResponse.message == "Too many active assignments")
            {
                return "Ошибка! Нельзя взять больше заданий";
            }
            else
            {
                return "Ошибка! " + takeTaskResponse.message;
            }
        }

        private static string ParseWebEnvironment(dynamic takeTaskResponse)
        {
            string environment;
            string json;
            using (var clientHttp = new HttpClient())
            {
                var result = clientHttp.GetAsync((string)takeTaskResponse.tasks[0].input_values.data.testrun_info.required_envs[0]).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            environment = $"Окружение: {Regex.Replace(Regex.Replace(json, @"<.*?>", " ").Replace("ИЛИ", "").Replace(" И ", "").Replace("&gt;", ">").Replace("&lt;", "<"), @"\s+", " ")}";
            return environment;
        }
    }
}
