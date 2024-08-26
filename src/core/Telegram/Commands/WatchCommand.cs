using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public class WatchCommand(TelegramBotClient bot, IFlatAdRepository flatAdRepository, ILogger<TelegramCommand> logger)
    : TelegramCommand(bot, "/watch", logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var url = msg.Text!.Split(" ")[1];
        if (!url.StartsWith("https://www.leboncoin.fr/"))
        {
            await _bot.SendTextMessageAsync(msg.Chat, "Invalid URL. Please provide a valid LeBonCoin search URL.",
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
            return;
        }

        var flatAds = await LeBonCoin.AdExtractor.GetAdsFromUrl(url);
        if (flatAdRepository.EntriesForURlAndUserExist(url, telegramUser))
        {
            await _bot.SendTextMessageAsync(msg.Chat, "Already watching ads for this url",
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }
        else
        {
            flatAdRepository.UpsertFlatAds(flatAds, telegramUser);
            Console.WriteLine("Starting to watch ads from " + url);
            await _bot.SendTextMessageAsync(msg.Chat, "Starting to watch ads from " + url,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }
    }
}