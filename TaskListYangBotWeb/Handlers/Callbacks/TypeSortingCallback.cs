﻿using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Helper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskListYangBotWeb.Handlers.Callbacks
{
    public class TypeSortingCallback : BaseHandler
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IUserRepository _userRepository;

        public TypeSortingCallback(TelegramBotService telegramBotHelper, IUserRepository userRepository)
        {
            _telegramBotClient = telegramBotHelper.GetBot().Result;
            _userRepository = userRepository;
        }
        public override string Name => CallbackNames.TypeSortingCallback;

        public async override Task ExecuteAsync(Update update)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Загрузка...");
            int indexSorting = Convert.ToInt32(update.CallbackQuery.Data.Replace(CallbackNames.TypeSortingCallback, ""));
            if (_userRepository.UpdateUserSorting(update.CallbackQuery.Message.Chat.Id, indexSorting))
                await _telegramBotClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    messageId: update.CallbackQuery.Message.MessageId,
                    replyMarkup: (InlineKeyboardMarkup)CreateButtons.GetButtonTypesSorting(StaticFields.TypesSorting, indexSorting));
        }
    }
}
