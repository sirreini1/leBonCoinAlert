using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public class ListCommand(TelegramBotClient bot, IFlatAdRepository flatAdRepository, ILogger<TelegramCommand> logger)
    : TelegramCommand(bot, "/list", logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
        var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().OrderByDescending(url => url).ToList();

        var message = "Your watched search URLs:\n";
        for (var i = 0; i < uniqueSearchUrls.Count; i++) message += $"Id: {i} URL: {uniqueSearchUrls[i]}\n";

        message += "To remove a url to watch type /remove followed by the id. For example /remove 1";

        if (uniqueSearchUrls.Count == 0) message = "You are not watching any search URLs";

        await _bot.SendTextMessageAsync(msg.Chat, message,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}