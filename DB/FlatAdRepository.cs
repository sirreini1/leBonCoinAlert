using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeBonCoinAlert.DB;

public class FlatAdEntity(string location, string description, string price, string url, long id = 0)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; } = id; // Unique identifier for each ad (list_id)

    [MaxLength(255)] public string Location { get; init; } = location;
    [MaxLength(1000)] public string Description { get; init; } = description;
    [MaxLength(10)] public string Price { get; init; } = price;

    [MaxLength(1000)] public string Url { get; set; } = url;

    public static FlatAdEntity FromFlatAd(FlatAd flatAd)
    {
        return new FlatAdEntity(flatAd.Location, flatAd.Description, flatAd.Price, flatAd.Url);
    }

    public static FlatAd ToFlatAd(FlatAdEntity flatAdEntity)
    {
        return new FlatAd(flatAdEntity.Location, flatAdEntity.Description, flatAdEntity.Price, flatAdEntity.Url);
    }
}

public class FlatAdRepository
{
    private readonly AppDbContext _context;

    public FlatAdRepository()
    {
        _context = new AppDbContext();
        _context.Database.EnsureCreated();
    }

    public void CreateFlatAd(FlatAd flatAd)
    {
        var entity = FlatAdEntity.FromFlatAd(flatAd);
        _context.FlatAds.Add(entity);
        _context.SaveChanges();
    }

    public List<FlatAd> ReadFlatAds()
    {
        return _context.FlatAds.Select(e => FlatAdEntity.ToFlatAd(e)).ToList();
    }

    public void UpdateFlatAd(FlatAd flatAd)
    {
        _context.FlatAds.Update(FlatAdEntity.FromFlatAd(flatAd));
        _context.SaveChanges();
    }

    public void DeleteFlatAd(string id)
    {
        var flatAd = _context.FlatAds.Find(id);
        if (flatAd != null)
        {
            _context.FlatAds.Remove(flatAd);
            _context.SaveChanges();
        }
    }

    public void DeleteAllFlatAds()
    {
        _context.FlatAds.RemoveRange(_context.FlatAds);
        _context.SaveChanges();
    }

    public void UpsertFlatAds(List<FlatAd> flatAds)
    {
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd);
            var existingAd = _context.FlatAds.Find(entity.Id);
            if (existingAd != null)
                _context.Entry(existingAd).CurrentValues.SetValues(entity);
            else
                _context.FlatAds.Add(entity);
        }

        _context.SaveChanges();
    }
}