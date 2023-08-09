using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_bot
{
    public class BotEngine
    {
        private TelegramBotClient botClient;
        GetFromApi fromApi = new GetFromApi("https://the-trivia-api.com/api/questions?limit=1");
        public BotEngine(string Api)
        {
            botClient = new TelegramBotClient(Api);
        }

        async public Task StartBot()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new();
            {
                var AllowedUpdates = Array.Empty<UpdateType>();
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start {me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;
            var chatId = message.Chat.Id;
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            if (message.Text == fromApi.GetCorAnswers())
            {
                Message sentMg = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Правильно",
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    cancellationToken: cancellationToken);
                return;
            }

            if (fromApi.GetIncAnswers().Contains(message.Text))
            {
                Message sentMg = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Соси жепу",
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    cancellationToken: cancellationToken);
                return;
            }

            // Echo received message text
            fromApi.get();
            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: fromApi.GetQuestion(),
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    cancellationToken: cancellationToken);


            ReplyKeyboardMarkup kb1 = new ReplyKeyboardMarkup(new KeyboardButton[]
            {
                new KeyboardButton(text: fromApi.GetCorAnswers()),
                new KeyboardButton(text: fromApi.GetIncAnswers()[0]),
                new KeyboardButton(text: fromApi.GetIncAnswers()[1]),
                new KeyboardButton(text: fromApi.GetIncAnswers()[2])
            });

            Message sentMs = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Ответы:",
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    replyMarkup: kb1,
                    cancellationToken: cancellationToken);

        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
