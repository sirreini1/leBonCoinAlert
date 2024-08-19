using LeBonCoinAlert.core;

namespace LeBonCoinAlert.DB;

public class FlatAdRepository(AppDbContext dbContext)
{
    public List<FlatAd> GetFlatAds()
    {
        return dbContext.FlatAds.Select(e => FlatAdEntity.ToFlatAd(e)).ToList();
    }

    public List<FlatAd> GetFlatAdsForUser(string telegramUser)
    {
        return dbContext.FlatAds.Where(e => e.TelegramUser == telegramUser).Select(e => FlatAdEntity.ToFlatAd(e))
            .ToList();
    }

    public void DeleteFlatAdsForUserAndUrl(string telegramUser, string searchUrl)
    {
        dbContext.FlatAds.RemoveRange(dbContext.FlatAds.Where(e =>
            e.TelegramUser == telegramUser && e.SearchUrl == searchUrl));
        dbContext.SaveChanges();
    }

    public List<TelegramUserSearchUrlPair> GetUniqueTelegramUserSearchUrlPairs()
    {
        return dbContext.FlatAds
            .GroupBy(flatAdEntity => new { flatAdEntity.TelegramUser, flatAdEntity.SearchUrl })
            .Select(group => new TelegramUserSearchUrlPair(group.Key.TelegramUser, group.Key.SearchUrl))
            .ToList();
    }
    

    public void DeleteFlatAd(string id)
    {
        var flatAd = dbContext.FlatAds.Find(id);
        if (flatAd != null)
        {
            dbContext.FlatAds.Remove(flatAd);
            dbContext.SaveChanges();
        }
    }

    public void DeleteAllFlatAds()
    {
        dbContext.FlatAds.RemoveRange(dbContext.FlatAds);
        dbContext.SaveChanges();
    }

    public void UpsertFlatAd(FlatAd flatAd, string telegramUser)
    {
        var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
        var existingAd = dbContext.FlatAds.Find(entity.Id);
        if (existingAd != null)
            dbContext.Entry(existingAd).CurrentValues.SetValues(entity);
        else
            dbContext.FlatAds.Add(entity);

        dbContext.SaveChanges();
    }

    public List<FlatAd> CheckForNewAds(List<FlatAd> flatAds, string telegramUser)
    {
        var newAds = new List<FlatAd>();
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = dbContext.FlatAds.Find(entity.Id);
            if (existingAd == null)
            {
                newAds.Add(flatAd);
            }
        }

        return newAds;
    }

    public bool EntriesForURlAndUserExist(string searchUrl, string telegramUser)
    {
        return dbContext.FlatAds.Any(e => e.SearchUrl == searchUrl && e.TelegramUser == telegramUser);
    }

    public void UpsertFlatAds(List<FlatAd> flatAds, string telegramUser)
    {
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = dbContext.FlatAds.Find(entity.Id);
            if (existingAd != null)
                dbContext.Entry(existingAd).CurrentValues.SetValues(entity);
            else
                dbContext.FlatAds.Add(entity);
        }

        dbContext.SaveChanges();
    }
}