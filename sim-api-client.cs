using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static string BotToken = "tg_token";
    private static List<string> ApiTokens = new List<string>
    {
        "ApiTokem1",
        "ApiToken2",
        "ApiToken3",
        "ApiToken4",
        "ApiToken5"
    };
    private static string RegistrationKey = "myregkey";

    private static TelegramBotClient botClient;
    private static bool isBotRunning = false;
    private static bool isRegistered = false;
    private static int currentApiIndex = 0;

    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient(BotToken);
        var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
        Console.WriteLine("Бот запущен.");

        Console.ReadLine();
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message.Type != MessageType.Text)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;

        switch (message.Text.ToLower())
        {
            case "/start":
                if (!isRegistered)
                {
                    await botClient.SendTextMessageAsync(chatId, "Пожалуйста, введите ключ для регистрации с помощью команды /register <ключ>", cancellationToken: cancellationToken);
                    return;
                }
                isBotRunning = true;
                await botClient.SendTextMessageAsync(chatId, "Бот начал принимать данные с токена.", cancellationToken: cancellationToken);
                break;

            case "/stop":
                isBotRunning = false;
                await botClient.SendTextMessageAsync(chatId, "Бот остановлен и не принимает сообщения.", cancellationToken: cancellationToken);
                break;

            case "/addnumber":
                if (!isBotRunning || !isRegistered)
                {
                    await botClient.SendTextMessageAsync(chatId, "Бот не запущен или не зарегистрирован.", cancellationToken: cancellationToken);
                    return;
                }

                currentApiIndex = (currentApiIndex + 1) % ApiTokens.Count;
                var currentApiToken = ApiTokens[currentApiIndex];

                await botClient.SendTextMessageAsync(chatId, $"API переключен на: {currentApiToken}", cancellationToken: cancellationToken);
                break;

            default:
                if (message.Text.StartsWith("/register"))
                {
                    var key = message.Text.Split(" ")[1];
                    if (key == RegistrationKey)
                    {
                        isRegistered = true;
                        await botClient.SendTextMessageAsync(chatId, "Регистрация успешна! Теперь вы можете использовать команды.", cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId, "Неверный ключ регистрации.", cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Неизвестная команда. Используйте /start, /stop, /addnumber или /register <ключ>.", cancellationToken: cancellationToken);
                }
                break;
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
