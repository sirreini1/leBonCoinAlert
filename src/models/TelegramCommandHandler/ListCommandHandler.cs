// src/commands/ListCommandHandler.cs

using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler
{
    public class ListCommandHandler(TelegramBotClient bot, IFlatAdRepository flatAdRepository) : ITelegramCommandHandler
    {
        public async Task HandleCommand(Message msg, CancellationTokenSource cts)
        {
            var telegramUser = msg.From!.Id.ToString();
            var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
            var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().OrderByDescending(url => url).ToList();

            var message = "Your watched search URLs:\n";
            for (var i = 0; i < uniqueSearchUrls.Count; i++)
            {
                message += $"Id: {i} URL: {uniqueSearchUrls[i]}\n";
            }

            message += $"To remove a url to watch type /remove followed by the id. For example /remove 1";

            if (uniqueSearchUrls.Count == 0)
            {
                message = "You are not watching any search URLs";
            }

            await bot.SendTextMessageAsync(msg.Chat, message, cancellationToken: cts.Token,
                linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
        }
    }
}