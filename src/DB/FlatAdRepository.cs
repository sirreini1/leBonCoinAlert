using LeBonCoinAlert.core;
using LeBonCoinAlert.models;

namespace LeBonCoinAlert.DB;

public interface IFlatAdRepository
{
    List<FlatAd> GetFlatAds();
    List<FlatAd> GetFlatAdsForUser(string telegramUser);
    void DeleteFlatAdsForUserAndUrl(string telegramUser, string searchUrl);
    List<TelegramUserSearchUrlPair> GetUniqueTelegramUserSearchUrlPairs();
    void DeleteFlatAd(string id);
    void DeleteAllFlatAds();
    void UpsertFlatAd(FlatAd flatAd, string telegramUser);
    List<FlatAd> CheckForNewAds(List<FlatAd> flatAds, string telegramUser);
    bool EntriesForURlAndUserExist(string searchUrl, string telegramUser);
    string GetStatisticPerUser(string telegramUser);
    void UpsertFlatAds(List<FlatAd> flatAds, string telegramUser);
}

public class FlatAdRepository : IFlatAdRepository
{
    private readonly AppDbContext _dbContext1;

    public FlatAdRepository(AppDbContext _dbContext)
    {
        _dbContext1 = _dbContext;
    }

    public List<FlatAd> GetFlatAds()
    {
        return _dbContext1.FlatAds.Select(e => FlatAdEntity.ToFlatAd(e)).ToList();
    }

    public List<FlatAd> GetFlatAdsForUser(string telegramUser)
    {
        return _dbContext1.FlatAds.Where(e => e.TelegramUser == telegramUser).Select(e => FlatAdEntity.ToFlatAd(e))
            .ToList();
    }

    public void DeleteFlatAdsForUserAndUrl(string telegramUser, string searchUrl)
    {
        _dbContext1.FlatAds.RemoveRange(_dbContext1.FlatAds.Where(e =>
            e.TelegramUser == telegramUser && e.SearchUrl == searchUrl));
        _dbContext1.SaveChanges();
    }

    public List<TelegramUserSearchUrlPair> GetUniqueTelegramUserSearchUrlPairs()
    {
        return _dbContext1.FlatAds
            .GroupBy(flatAdEntity => new { flatAdEntity.TelegramUser, flatAdEntity.SearchUrl })
            .Select(group => new TelegramUserSearchUrlPair(group.Key.TelegramUser, group.Key.SearchUrl))
            .ToList();
    }


    public void DeleteFlatAd(string id)
    {
        var flatAd = _dbContext1.FlatAds.Find(id);
        if (flatAd != null)
        {
            _dbContext1.FlatAds.Remove(flatAd);
            _dbContext1.SaveChanges();
        }
    }

    public void DeleteAllFlatAds()
    {
        _dbContext1.FlatAds.RemoveRange(_dbContext1.FlatAds);
        _dbContext1.SaveChanges();
    }

    public void UpsertFlatAd(FlatAd flatAd, string telegramUser)
    {
        var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
        var existingAd = _dbContext1.FlatAds.Find(entity.Id);
        if (existingAd != null)
            _dbContext1.Entry(existingAd).CurrentValues.SetValues(entity);
        else
            _dbContext1.FlatAds.Add(entity);

        _dbContext1.SaveChanges();
    }

    public List<FlatAd> CheckForNewAds(List<FlatAd> flatAds, string telegramUser)
    {
        var newAds = new List<FlatAd>();
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = _dbContext1.FlatAds.Find(entity.Id);
            if (existingAd == null)
            {
                newAds.Add(flatAd);
            }
        }

        return newAds;
    }

    public bool EntriesForURlAndUserExist(string searchUrl, string telegramUser)
    {
        return _dbContext1.FlatAds.Any(e => e.SearchUrl == searchUrl && e.TelegramUser == telegramUser);
    }

    public string GetStatisticPerUser(string telegramUser)
    {
        var userAds = _dbContext1.FlatAds.Where(e => e.TelegramUser == telegramUser).ToList();
        var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().ToList();
        var message = $"You are watching {uniqueSearchUrls.Count} unique search URLs. With:\n\n";

        for (var i = 0; i < uniqueSearchUrls.Count; i++)
        {
            var url = uniqueSearchUrls[i];
            var adsForUrl = userAds.Where(ad => ad.SearchUrl == url).ToList();
            message += $"{adsForUrl.Count} ads for url with id: {i}\n";
        }

        return message;
    }

    public void UpsertFlatAds(List<FlatAd> flatAds, string telegramUser)
    {
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = _dbContext1.FlatAds.Find(entity.Id);
            if (existingAd != null)
                _dbContext1.Entry(existingAd).CurrentValues.SetValues(entity);
            else
                _dbContext1.FlatAds.Add(entity);
        }

        _dbContext1.SaveChanges();
    }
}