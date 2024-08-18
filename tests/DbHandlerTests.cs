using LeBonCoinAlert.DB;
using Xunit;

namespace LeBonCoinAlert.tests;

public class DbHandlerTests : IDisposable
{
    private readonly FlatAdRepository _flatAdRepository = new();

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
            new("Location 1", "Ad 1", "450", "url"),
            new("Location 2", "Ad 2", "500", "url")
        };

        _flatAdRepository.UpsertFlatAds(ads);

        // Act
        _flatAdRepository.DeleteAllFlatAds();
        var result = _flatAdRepository.ReadFlatAds();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void UpsertFlatAds_ShouldInsertOrUpdateAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450", "url"),
            new("Location 2", "Ad 2", "500", "url")
        };

        // Act
        _flatAdRepository.UpsertFlatAds(ads);
        var result = _flatAdRepository.ReadFlatAds();

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
            new("Location 1", "Ad 1", "450", "url"),
            new("Location 2", "Ad 2", "500", "url")
        };

        _flatAdRepository.UpsertFlatAds(ads);

        // Act
        var result = _flatAdRepository.ReadFlatAds();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Ad 1", result[0].Description);
        Assert.Equal("Ad 2", result[1].Description);
    }
}