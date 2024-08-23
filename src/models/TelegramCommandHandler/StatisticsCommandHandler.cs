// src/commands/StatisticsCommandHandler.cs

using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler;

public class StatisticsCommandHandler(TelegramBotClient bot, IFlatAdRepository flatAdRepository)
    : ITelegramCommandHandler
{
    public async Task HandleCommand(Message msg, CancellationTokenSource cts)
    {
        var telegramUser = msg.From!.Id.ToString();
        var message = flatAdRepository.GetStatisticPerUser(telegramUser);
        await bot.SendTextMessageAsync(msg.Chat, message, cancellationToken: cts.Token);
    }
}