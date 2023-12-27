using TaskListYangBotWeb.Handlers;
using TaskListYangBotWeb.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb
{
    public class CreateButtons
    {
        public static IReplyMarkup GetButton(dynamic takeTaskResponse)
        {
            IReplyMarkup replyMarkup;
            string linkTask = StaticFields.linkTask + takeTaskResponse.poolId + "/" + takeTaskResponse.id;
            string checkEnvironment = takeTaskResponse.tasks[0].input_values.data.testrun_info.env_requester_code_explanation != null

                ? takeTaskResponse.tasks[0].input_values.data.testrun_info.env_requester_code_explanation[0]
                : "";
            string urlTestStand = takeTaskResponse.tasks[0].input_values.data.version_info.test_stend;

            if (checkEnvironment == "" && !Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute))
            {
                replyMarkup = GetButton((int)takeTaskResponse.poolId, "Выйти", "Ссылка на задание", linkTask);
            }
            else if (Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute) && checkEnvironment == "")
            {
                replyMarkup = GetButton((int)takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на тестовый стенд", linkTask, urlTestStand);
            }
            else if (!Uri.IsWellFormedUriString(urlTestStand, UriKind.Absolute) && checkEnvironment != "")
            {
                replyMarkup = GetButton((int)takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на проверку окружения", linkTask, checkEnvironment);
            }
            else
            {
                replyMarkup = GetButton((int)takeTaskResponse.poolId, "Выйти", "Ссылка на задание", "Ссылка на проверку окружения", "Ссылка на тестовый стенд", linkTask, checkEnvironment, urlTestStand);
            }

            return replyMarkup;
        }

        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2 } }
            };
            return new InlineKeyboardMarkup(keyboardButton);
        }

        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string url)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = url } }
            };
            return new InlineKeyboardMarkup(keyboardButton);
        }

        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string textButton3, string urlFor2, string urlFor3)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = urlFor2 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton3) { CallbackData = id.ToString() + textButton3, Url = urlFor3 } }

            };
            return new InlineKeyboardMarkup(keyboardButton);
        }

        public static IReplyMarkup GetButton(int id, string textButton1, string textButton2, string textButton3, string textButton4, string urlFor2, string urlFor3, string urlTestStand)
        {
            InlineKeyboardButton[][] keyboardButton = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton1) { CallbackData = id.ToString() + textButton1 }, new InlineKeyboardButton(textButton2) { CallbackData = id.ToString() + textButton2, Url = urlFor2 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton3) { CallbackData = id.ToString() + textButton3, Url = urlFor3 } },
                new InlineKeyboardButton[] { new InlineKeyboardButton(textButton4) { CallbackData = id.ToString() + textButton4, Url = urlTestStand } }

            };
            return new InlineKeyboardMarkup(keyboardButton);
        }

        public static IReplyMarkup GetButton(List<string> textButtons)
        {
            List<InlineKeyboardButton[]> inlineKeyboard = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < textButtons.Count; i++)
            {
                inlineKeyboard.Add(new InlineKeyboardButton[] { new InlineKeyboardButton(textButtons[i]) });
                inlineKeyboard[i][0].CallbackData = CallbackNames.TypeSortingCallback + i.ToString();
            }

            return new InlineKeyboardMarkup(inlineKeyboard.ToArray());
        }

        public static IReplyMarkup GetButtonsFavoriteTasks(List<FavoriteTask> favoriteTasks)
        {
            List<InlineKeyboardButton[]> inlineKeyboard = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < favoriteTasks.Count; i++)
            {
                inlineKeyboard.Add(new InlineKeyboardButton[] { new InlineKeyboardButton(favoriteTasks[i].TaskName) });
                inlineKeyboard[i][0].CallbackData = CallbackNames.FavoriteTaskCallback + favoriteTasks[i].PoolId;
            }

            // Keyboard
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(inlineKeyboard.ToArray());

            return keyboard;
        }
    }
}
