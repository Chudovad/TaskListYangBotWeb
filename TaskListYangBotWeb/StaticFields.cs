﻿using Microsoft.Extensions.Configuration;
using TaskListYangBotWeb.Data.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangTgBot
{
    internal class StaticFields
    {
        //public readonly static string passwordEncryption = new ConfigurationManager().GetValue<string>("PasswordEncryption");
        //public readonly static string linkTask = new ConfigurationManager().GetValue<string>("UrlLinkTask");
        //public readonly static string linkManual = new ConfigurationManager().GetValue<string>("UrlLinkManual");
        public static string passwordEncryption { get; set; }
        public static string linkTask { get; set; }
        public static string linkManual { get; set; }

        private static string commandMsg = "Список команд:\n/yang \\- список заданий из Янг 📋\n/atwork \\- задания в работе 🗺\n/yangon \\- ждёт пока придут задания и берет первое 🔖" +
            "\n/yangonfavorite \\- ждёт пока придут любимые задания и берет первое ⭐️\n/favoritetasks \\- удалить или добавить в список любимых заданий ❤️" +
            "\n/tasks\\_sorting \\- выбор сортировки заданий 📈\n/norma \\- недельная норма 💸\n/help \\- Как пользоваться ботом❔";

        public static string CommandMsg { get { return commandMsg; } }

        private static string getTokenMsg = "Пришли свой OAuth токен Янга и бот будет работать\\. \r";

        public static string GetTokenMsg { get { return getTokenMsg; } }

        private static string linkToManual = $"\nТокен можно найти в [инструкции]({linkManual})";

        public static string LinkToManual { get { return linkToManual; } }

        private static string helpMsg = " Этот телеграмм\\-бот предназначен для автоматизации процесса взятия заданий\\. " +
            "С помощью бота вы сможете просматривать список доступных заданий, выбирать фильтр отображения заданий, брать их в работу, отслеживать задания, которые уже в работе, " +
            "а также добавлять задания в список любимых, чтобы в дальнейшем брать только любимые задания\\. " +
            "После взятия задания вы сможет выйти из него, перейти по ссылке на само задание и проверить свое окружение, перейдя по ссылке\\. " +
            "Бот облегчит вашу работу, сделав процесс взятия заданий более эффективным и удобным\\." +
            "\nКогда вы отправляете боту ссылку, он отправит вам сообщение, при нажатии на которое ссылка будет скопирована в буфер обмена вашего устройства\\.\n" +
            "\n" + CommandMsg +
            "\r\n\r\nПришли свой OAuth токен Янг и бот будет работать\\. \r" + LinkToManual;

        public static string HelpMsg { get { return helpMsg; } }

        private static string removeMsg = "Нажмите на кнопку с названием задания чтобы удалить его из списка любимых.";

        public static string RemoveMsg { get { return removeMsg; } }

        private static string favoriteTaskMsg = "\nОтправьте название любимого задания в ответ на это сообщение и бот добавит его в ваш список." +
                    "\nПри использовании команды /yangonfavorite будет браться первое задание на столе из вашего списка любимых заданий.";

        public static string FavoriteTaskMsg { get { return favoriteTaskMsg; } }

        private static List<string> typesSorting = new List<string> { "по цене(убывание)", "по цене(возрастание)", "без сортировки" };

        public static List<string> TypesSorting { get { return typesSorting; } set { typesSorting = value; } }

        private readonly static ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup("Завершить команду")
        {
            ResizeKeyboard = true
        };

        private static double numberOfAddedHoursForServer = 3;

        public static double NumberOfAddedHoursForServer { get { return numberOfAddedHoursForServer; } }

        public static ReplyKeyboardMarkup Keyboard { get { return keyboard; } }

        public static readonly int countAddHours = 3;

        private static ReplyKeyboardMarkup keyboardForYangCommand = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Ещё 20 заданий"),
                        new KeyboardButton("Ещё 50 заданий")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Завершить")
                    }
                })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup KeyboardForYangCommand { get { return keyboardForYangCommand; } }

        public static string GetTaskSortingText(int typeSorting)
        {
            return $"Стоит сортировка: {StaticFields.TypesSorting[typeSorting]}\r\nВыбери сортировку заданий. " +
                $"По этой сортировке задания будут отображаться в команде /yang. " +
                $"В командах /yangon и /yangonfavorite будет браться первое задание в соответствии выбранной сортировке.";
        }
    }
}
