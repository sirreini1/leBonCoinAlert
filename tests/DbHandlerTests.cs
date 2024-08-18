using Xunit;

namespace LeBonCoinAlert.tests;

public class DbHandlerTests : IDisposable
{
    private readonly DbHandler _dbHandler = new();

    public void Dispose()
    {
        _dbHandler.DeleteAllFlatAds();
    }

    [Fact]
    public void DeleteAllFlatAds_ShouldDeleteAllAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450"),
            new("Location 2", "Ad 2", "500")
        };

        _dbHandler.UpsertFlatAds(ads);

        // Act
        _dbHandler.DeleteAllFlatAds();
        var result = _dbHandler.ReadFlatAds();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void UpsertFlatAds_ShouldInsertOrUpdateAds()
    {
        // Arrange
        var ads = new List<FlatAd>
        {
            new("Location 1", "Ad 1", "450"),
            new("Location 2", "Ad 2", "500")
        };

        // Act
        _dbHandler.UpsertFlatAds(ads);
        var result = _dbHandler.ReadFlatAds();

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
            new("Location 1", "Ad 1", "450"),
            new("Location 2", "Ad 2", "500")
        };

        _dbHandler.UpsertFlatAds(ads);

        // Act
        var result = _dbHandler.ReadFlatAds();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Ad 1", result[0].Description);
        Assert.Equal("Ad 2", result[1].Description);
    }
}