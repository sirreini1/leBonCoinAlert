// src/commands/RemoveCommandHandler.cs

using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler
{
    public class RemoveCommandHandler(TelegramBotClient bot, IFlatAdRepository flatAdRepository)
        : ITelegramCommandHandler
    {
        public async Task HandleCommand(Message msg, CancellationTokenSource cts)
        {
            var telegramUser = msg.From!.Id.ToString();
            try
            {
                var id = int.Parse(msg.Text!.Split(" ")[1]);
                var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
                var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().OrderByDescending(url => url).ToList();

                if (id < 0 || id > uniqueSearchUrls.Count)
                {
                    await bot.SendTextMessageAsync(msg.Chat, "Invalid id", cancellationToken: cts.Token);
                    return;
                }

                var searchUrl = uniqueSearchUrls[id];
                flatAdRepository.DeleteFlatAdsForUserAndUrl(telegramUser, searchUrl);
                await bot.SendTextMessageAsync(msg.Chat, $"Removed search url with id: {id}", cancellationToken: cts.Token);
            }
            catch (Exception e)
            {
                await bot.SendTextMessageAsync(msg.Chat, "Invalid id", cancellationToken: cts.Token);
                Console.WriteLine("Error removing search url: " + e.Message);
                throw;
            }
        }
    }
}