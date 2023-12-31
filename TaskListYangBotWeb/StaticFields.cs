﻿using Microsoft.Extensions.Configuration;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Handlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb
{
    internal class StaticFields
    {
        public static string passwordEncryption { get; set; }
        public static string linkTask { get; set; }
        public static string linkManual { get; set; }

        public static string urlTaskList { get; set; }
        public static string urlTakeTask { get; set; }
        public static string urlLeaveTask { get; set; }
        public static string urlCheckToken { get; set; }
        public static string urlTaskTitle { get; set; }
        public static string urlCheckNorm { get; set; }

        private static string commandMsg = $"Список команд:\n{CommandNames.YangCommand} \\- список заданий из Янг 📋\n{CommandNames.AtWorkCommand} \\- задания в работе 🗺\n{CommandNames.YangOnCommand} \\- ждёт пока придут задания и берет первое 🔖" +
            $"\n{CommandNames.YangOnFavoriteCommand} \\- ждёт пока придут любимые задания и берет первое ⭐️\n{CommandNames.FavoriteTasksCommand} \\- удалить или добавить в список любимых заданий ❤️" +
            $"\n{CommandNames.TasksSortingCommand.Replace("_", "\\_")} \\- выбор сортировки заданий 📈\n{CommandNames.NormaCommand} \\- недельная норма 💸\n{CommandNames.HelpCommand} \\- Как пользоваться ботом❔";

        public static string CommandMsg { get { return commandMsg; } }

        private static string getTokenMsg = "Отправь в ответ на это сообщение свой OAuth токен Янга, и бот будет работать\\. \r";

        public static string GetTokenMsg { get { return getTokenMsg + LinkToManual; } }

        private static string linkToManual = "\nТокен можно найти в [инструкции](";

        public static string LinkToManual { get { return linkToManual + linkManual + ")"; } }

        private static string helpMsg = " Этот телеграмм\\-бот предназначен для автоматизации процесса взятия заданий\\. " +
            "С помощью бота вы сможете просматривать список доступных заданий, выбирать фильтр отображения заданий, брать их в работу, отслеживать задания, которые уже в работе, " +
            "а также добавлять задания в список любимых, чтобы в дальнейшем брать только любимые задания\\. " +
            "После взятия задания вы сможет выйти из него, перейти по ссылке на само задание и проверить свое окружение, перейдя по ссылке\\. " +
            "Бот облегчит вашу работу, сделав процесс взятия заданий более эффективным и удобным\\." +
            "\nКогда вы отправляете боту ссылку, он отправит вам сообщение, при нажатии на которое ссылка будет скопирована в буфер обмена вашего устройства\\.\n" +
            "\n" + CommandMsg;

        public static string HelpMsg { get { return helpMsg; } }

        private static string removeMsg = "Нажмите на кнопку с названием задания чтобы удалить его из списка любимых.";

        public static string RemoveMsg { get { return removeMsg; } }

        private static string favoriteTaskMsg = "\nОтправьте название любимого задания в ответ на это сообщение и бот добавит его в ваш список." +
                    $"\nПри использовании команды {CommandNames.YangOnFavoriteCommand} будет браться первое задание на столе из вашего списка любимых заданий.";

        public static string FavoriteTaskMsg { get { return favoriteTaskMsg; } }

        private static List<string> typesSorting = new List<string> { "по цене(убывание)", "по цене(возрастание)", "без сортировки" };

        public static List<string> TypesSorting { get { return typesSorting; } set { typesSorting = value; } }

        private readonly static ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(KeyboardCommandNames.CompleteYangOnCommandKeyboard)
        {
            ResizeKeyboard = true
        };

        private static double numberOfAddedHoursForServer = 3;

        public static double NumberOfAddedHoursForServer { get { return numberOfAddedHoursForServer; } }

        public static ReplyKeyboardMarkup Keyboard { get { return keyboard; } }

        public static readonly int countAddHours = 3;

        private static ReplyKeyboardMarkup keyboardForYangCommand = 
            new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton(KeyboardCommandNames.GetTwentyTaskKeyboard)
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton(KeyboardCommandNames.CompleteYangCommandKeyboard)
                    }
                })
                {
                    ResizeKeyboard = true
                };

        public static ReplyKeyboardMarkup KeyboardForYangCommand { get { return keyboardForYangCommand; } }

        public static string GetTaskSortingText(int typeSorting)
        {
            return $"Выбери сортировку заданий. " +
                $"По этой сортировке задания будут отображаться в команде {CommandNames.YangCommand}. " +
                $"В командах {CommandNames.YangOnCommand} и {CommandNames.YangOnFavoriteCommand} будет браться первое задание в соответствии выбранной сортировке.";
        }
    }
}
