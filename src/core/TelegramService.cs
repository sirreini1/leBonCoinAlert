// src/core/TelegramHandler.cs

using LeBonCoinAlert.DB;
using LeBonCoinAlert.models.TelegramCommandHandler;
using LeBonCoinAlert.models.TelegramCommands;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core;

public interface ITelegramService
{
    Task SendMessageToUser(string userId, string message);
}

public class TelegramService(IUserPairChatIdRepository userPairChatIdRepository, IFlatAdRepository flatAdRepository)
    : ITelegramService
{
    private readonly TelegramBotClient _bot = CreateBot(flatAdRepository, userPairChatIdRepository);

    public async Task SendMessageToUser(string userId, string message)
    {
        var userChatIdPair = userPairChatIdRepository.FindChatByUserId(userId);
        if (userChatIdPair?.ChatId != null)
            await _bot.SendTextMessageAsync(userChatIdPair.ChatId, message);
        else
            Console.WriteLine("User not found, could not send message");
    }

    private static TelegramBotClient CreateBot(IFlatAdRepository flatAdRepository,
        IUserPairChatIdRepository userPairChatIdRepository)
    {
        var botToken = Environment.GetEnvironmentVariable("TOKEN");
        if (botToken is null) throw new Exception("No bot token found in environment variables");

        var cts = new CancellationTokenSource();
        var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
        bot.OnError += OnError;
        bot.OnMessage += OnMessage;
        bot.OnUpdate += OnUpdate;

        Console.WriteLine("Bot started");

        return bot;

        Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        async Task OnUpdate(Update update)
        {
            await bot.SendTextMessageAsync(update.Message!.Chat,
                "Curently we only support watching ads from leboncoin.fr by typing /watch and the search url",
                cancellationToken: cts.Token);
        }

        async Task OnMessage(Message? msg, UpdateType type)
        {
            if (msg?.Text is null)
            {
                Console.WriteLine("Error no message");
                return;
            }

            var msgText = msg.Text.ToLower();

            var telegramUser = msg.From!.Id.ToString();
            var chatId = msg.Chat.Id.ToString();
            userPairChatIdRepository.SaveUserChatIdPair(new UserChatIdPair(telegramUser, chatId));

            var commands = new List<ITelegramCommand>
            {
                new TelegramCommand("/help", new HelpCommandHandler(bot)),
                new TelegramCommand("/start", new StartCommandHandler(bot)),
                new TelegramCommand("/list", new ListCommandHandler(bot, flatAdRepository)),
                new TelegramCommand("/remove", new RemoveCommandHandler(bot, flatAdRepository)),
                new TelegramCommand("/statistics", new StatisticsCommandHandler(bot, flatAdRepository)),
                new TelegramCommand("/watch", new WatchCommandHandler(bot, flatAdRepository))
            };

            var command = commands.FirstOrDefault(c => msgText.StartsWith(c.CommandString));
            if (command?.CommandString != null)
                await command.CommandHandler.HandleCommand(msg, cts);
            else
                await bot.SendTextMessageAsync(msg.Chat, "Invalid command", cancellationToken: cts.Token);
        }
    }
}