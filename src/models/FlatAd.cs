namespace LeBonCoinAlert.models;

public class FlatAd(string location, string description, string price, string searchUrl, string adUrl)
{
    public FlatAd(FlatAdDto details, string searchUrl) : this(details.Location, details.Description,
        details.Price, searchUrl, details.AdUrl)
    {
    }

    public string Location { get; set; } = location;
    public string AdUrl { get; set; } = adUrl;
    public string Description { get; set; } = description;
    public string Price { get; set; } = price;

    public string SearchUrl { get; set; } = searchUrl;
}