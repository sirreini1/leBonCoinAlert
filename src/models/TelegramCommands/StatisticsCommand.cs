using LeBonCoinAlert.DB.repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.models.TelegramCommands;

public class StatisticsCommand(TelegramBotClient bot, IFlatAdRepository flatAdRepository) : TelegramCommand(bot, "/statistics")
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var message = flatAdRepository.GetStatisticPerUser(telegramUser);
        await _bot.SendTextMessageAsync(msg.Chat, message,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}