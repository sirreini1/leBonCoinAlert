using LeBonCoinAlert.DB.repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.models.TelegramCommands;

public class RemoveCommand(TelegramBotClient bot, IFlatAdRepository flatAdRepository) : TelegramCommand(bot, "/remove")
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        try
        {
            var id = int.Parse(msg.Text!.Split(" ")[1]);
            var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
            var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().OrderByDescending(url => url).ToList();

            if (id < 0 || id >= uniqueSearchUrls.Count)
            {
                await _bot.SendTextMessageAsync(msg.Chat, "Invalid id",
                    linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
                return;
            }

            var searchUrl = uniqueSearchUrls[id];
            flatAdRepository.DeleteFlatAdsForUserAndUrl(telegramUser, searchUrl);
            await _bot.SendTextMessageAsync(msg.Chat, $"Removed search url with id: {id}",
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }
        catch (Exception e)
        {
            await _bot.SendTextMessageAsync(msg.Chat, "Invalid id",
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
            Console.WriteLine("Error removing search url: " + e.Message);
            throw;
        }
    }
}