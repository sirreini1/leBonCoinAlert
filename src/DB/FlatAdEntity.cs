using System.ComponentModel.DataAnnotations;
using LeBonCoinAlert.core;
using LeBonCoinAlert.models;

namespace LeBonCoinAlert.DB;

public class FlatAdEntity(
    string location,
    string description,
    string price,
    string searchUrl,
    string adUrl,
    string telegramUser,
    string Id = "")

{
    [Key] [MaxLength(1000)] public string Id { get; init; } = GenerateId(telegramUser, adUrl);
    [MaxLength(1000)] public string AdUrl { get; init; } = adUrl;

    [MaxLength(256)] public string TelegramUser { get; init; } = telegramUser;

    [MaxLength(255)] public string Location { get; init; } = location;
    [MaxLength(1000)] public string Description { get; init; } = description;
    [MaxLength(10)] public string Price { get; init; } = price;
    [MaxLength(1000)] public string SearchUrl { get; init; } = searchUrl;

    //For some reason we need this?
    public FlatAdEntity() : this("", "", "", "", "", "")
    {
    }

    public static FlatAdEntity FromFlatAd(FlatAd flatAd, string telegramUser)
    {
        return new FlatAdEntity(flatAd.Location, flatAd.Description, flatAd.Price, flatAd.SearchUrl, flatAd.AdUrl,
            telegramUser);
    }

    public static string GenerateId(string telegramUser, string adUrl)
    {
        return $"{telegramUser}_{adUrl}";
    }


    public static FlatAd ToFlatAd(FlatAdEntity flatAdEntity)
    {
        return new FlatAd(flatAdEntity.Location, flatAdEntity.Description, flatAdEntity.Price, flatAdEntity.SearchUrl,
            flatAdEntity.AdUrl);
    }
}