using LeBonCoinAlert.DB;
using LeBonCoinAlert.models;

namespace LeBonCoinAlert.core;

public interface ISearchCheckService
{
    Task CheckForNewAdsPeriodically();
}

public class SearchCheckService(IFlatAdRepository flatAdRepository, ITelegramService telegramService)
    : ISearchCheckService
{
    public async Task CheckForNewAdsPeriodically()
    {
        while (true)
        {
            await CheckForNewAds();
            await Task.Delay(TimeSpan.FromMinutes(1)); // Wait for 1 minute
        }
    }

    private async Task CheckForNewAds()
    {
        var searchPairs = flatAdRepository.GetUniqueTelegramUserSearchUrlPairs();

        foreach (var pair in searchPairs)
        {
            Console.WriteLine("Checking for new ads for user: " + pair.TelegramUser);
            var flatAds = await AdExtractor.GetAdsFromUrl(pair.SearchUrl);
            var newAds = flatAdRepository.CheckForNewAds(flatAds, pair.TelegramUser);
            if (newAds.Count != 0)
            {
                Console.WriteLine("New ads found for user: " + pair.TelegramUser);
                foreach (var message in newAds.Select(GetFormattedMessage))
                    _ = telegramService.SendMessageToUser(pair.TelegramUser, message);
            }

            flatAdRepository.UpsertFlatAds(newAds, pair.TelegramUser);
        }
    }

    private static string GetFormattedMessage(FlatAd ad)
    {
        const string leBonCoinBaseUrl = "https://www.leboncoin.fr";
        var message = $"""
                       description: {ad.Description}
                       price: {ad.Price}
                       location: {ad.Location}
                       url: {leBonCoinBaseUrl}{ad.AdUrl}
                       """;

        return message;
    }
}