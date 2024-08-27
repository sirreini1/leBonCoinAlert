using System.Text.RegularExpressions;
using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public partial class StatisticsCommand(
    TelegramBotClient bot,
    IFlatAdRepository flatAdRepository,
    ILogger<StatisticsCommand> logger)
    : TelegramCommand(bot, StatisticsRegex(), logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var message = flatAdRepository.GetStatisticPerUser(telegramUser);
        logger.LogInformation($"User {telegramUser} requested statistics");
        await _bot.SendTextMessageAsync(msg.Chat, message,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }

    [GeneratedRegex("/statistics", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex StatisticsRegex();
}