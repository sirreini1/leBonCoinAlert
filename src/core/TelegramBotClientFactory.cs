using Telegram.Bot;

namespace LeBonCoinAlert.core;

public class TelegramBotClientFactory
{
    public static TelegramBotClient CreateBotClient()
    {
        var botToken = Environment.GetEnvironmentVariable("TOKEN");
        if (botToken is null) throw new Exception("No bot token found in environment variables");

        var cts = new CancellationTokenSource();
        return new TelegramBotClient(botToken, cancellationToken: cts.Token);
    }
}