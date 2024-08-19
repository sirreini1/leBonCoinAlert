using LeBonCoinAlert.DB;

namespace LeBonCoinAlert.core;

public class AddChecker(FlatAdRepository flatAdRepository, TelegramHandler telegramHandler)
{
    public async Task CheckForNewAdsPeriodically()
    {
        while (true)
        {
            await CheckForNewAds();
            await Task.Delay(TimeSpan.FromMinutes(1)); // Wait for 1 minute
        }
    }

    async Task CheckForNewAds()
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
                {
                    _ = telegramHandler.SendMessageToUser(pair.TelegramUser, message);
                }
            }

            flatAdRepository.UpsertFlatAds(newAds, pair.TelegramUser);
        }
    }

    string GetFormattedMessage(FlatAd ad)
    {
        const string leBonCoinBaseUrl = "https://www.leboncoin.fr";
        var message = $"""
                       description: {ad.Description}
                       price: {ad.Price}
                       location: {ad.Location})
                       url: {leBonCoinBaseUrl}{ad.AdUrl}
                       """;

        return message;
    }
}