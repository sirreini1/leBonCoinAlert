using LeBonCoinAlert.DB;
using LeBonCoinAlert.models;
using Xunit;

namespace LeBonCoinAlert.tests;

public class DbHandlerTests : IDisposable
{
    private readonly FlatAdRepository _flatAdRepository = new(new AppDbContext());

    public void Dispose()
    {
        _flatAdRepository.DeleteAllFlatAds();
    }

    [Fact]
    public void DeleteAllFlatAds_ShouldDeleteAllAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450", "url", "extraParam"),
            new("Location 2", "Ad 2", "500", "url", "extraParam")
        };
        var telegramUser = "telegramuser";
        _flatAdRepository.UpsertFlatAds(ads, telegramUser);

        // Act
        _flatAdRepository.DeleteAllFlatAds();
        var result = _flatAdRepository.GetFlatAds();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void UpsertFlatAds_ShouldInsertOrUpdateAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450", "url", "extraParam"),
            new("Location 2", "Ad 2", "500", "url", "extraParam2")
        };
        var telegramUser = "telegramuser";

        // Act
        _flatAdRepository.UpsertFlatAds(ads, telegramUser);
        var result = _flatAdRepository.GetFlatAds();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Ad 1", result[0].Description);
        Assert.Equal("Ad 2", result[1].Description);
    }

    [Fact]
    public void ReadFlatAds_ShouldReturnAllAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450", "url", "extraParam"),
            new("Location 2", "Ad 2", "500", "url", "extraParam2")
        };
        var telegramUser = "telegramuser";

        _flatAdRepository.UpsertFlatAds(ads, telegramUser);

        // Act
        var result = _flatAdRepository.GetFlatAds();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Ad 1", result[0].Description);
        Assert.Equal("Ad 2", result[1].Description);
    }
}