// src/commands/WatchCommandHandler.cs

using LeBonCoinAlert.core;
using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler
{
    public class WatchCommandHandler(TelegramBotClient bot, IFlatAdRepository flatAdRepository)
        : ITelegramCommandHandler
    {
        public async Task HandleCommand(Message msg, CancellationTokenSource cts)
        {
            var telegramUser = msg.From!.Id.ToString();
            var url = msg.Text.Split(" ")[1];
            if (!url.StartsWith("https://www.leboncoin.fr/"))
            {
                await bot.SendTextMessageAsync(msg.Chat, "Invalid URL. Please provide a valid LeBonCoin search URL.", cancellationToken: cts.Token);
                return;
            }

            var flatAds = await AdExtractor.GetAdsFromUrl(url);
            if (flatAdRepository.EntriesForURlAndUserExist(url, telegramUser))
            {
                await bot.SendTextMessageAsync(msg.Chat, "Already watching ads for this url", cancellationToken: cts.Token,
                    linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
            }
            else
            {
                flatAdRepository.UpsertFlatAds(flatAds, telegramUser);
                Console.WriteLine("Starting to watch ads from " + url);
                await bot.SendTextMessageAsync(msg.Chat, "Starting to watch ads from " + url, cancellationToken: cts.Token,
                    linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
            }
        }
    }
}