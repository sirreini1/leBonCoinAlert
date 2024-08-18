using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LeBonCoinAlert;

public class FlatAd(string location, string description, string price, string searchUrl, string adUrl)
{
    public string Location { get; set; } = location;
    public string adUrl { get; set; } = adUrl;
    public string Description { get; set; } = description;
    public string Price { get; set; } = price;

    public string SearchUrl { get; set; } = searchUrl;

    public FlatAd(FlatAdDto details, string searchUrl) : this(details.Location, details.Description,
        details.Price, searchUrl, details.AdUrl)
    {
    }
}

public record FlatAdDto(string Location, string Description, string Price, string AdUrl);

public static partial class AdExtractor
{
    public static async Task<List<FlatAd>> GetAdsFromUrl(string searchUrl)
    {
        var content = await Scraper.FetchPageContent(searchUrl);
        var adNodes = GetListingNodes(content);
        var flatAdDtos = adNodes.Select(ExtractAdDetails).ToList();
        return flatAdDtos.Select(dto => new FlatAd(dto, searchUrl)).ToList();
    }

    private static List<HtmlNode> GetListingNodes(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var aTagNodes = htmlDoc.DocumentNode.SelectNodes("//a[starts-with(@data-test-id, 'ad')]")
            .Where(node => node.InnerText.Contains('€'))
            .Where(node => !node.InnerHtml.Contains("Sponsorisé"))
            .Where(node => !node.InnerHtml.Contains("advertising"))
            .Where(node => !node.ParentNode.InnerHtml.Contains("Sponsorisé"))
            .Where(node => !node.ParentNode.InnerHtml.Contains("advertising"))
            .ToList();


        return aTagNodes;
    }


    private static FlatAdDto ExtractAdDetails(HtmlNode node)
    {
        var details = node.SelectNodes(".//p")
            .Select(htmlNode => htmlNode.InnerText).ToArray();
        var price = PriceRegex().Replace(details[0], string.Empty);
        var adUrl = node.Attributes.First(attr => attr.Name == "href").Value;
        return new FlatAdDto(details[2], details[1], price, adUrl);
    }

    [GeneratedRegex(@"[^0-9,.\u20AC]")]
    private static partial Regex PriceRegex();
}