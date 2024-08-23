using System.Text.RegularExpressions;
using HtmlAgilityPack;
using LeBonCoinAlert.models;

namespace LeBonCoinAlert.core;

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
        if (htmlContent == null) throw new ArgumentNullException(nameof(htmlContent), "HTML content cannot be null.");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var aTagNodes = htmlDoc.DocumentNode.SelectNodes("//a[starts-with(@href, '/ad/')]")
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

        var originalPriceDetail = details.FirstOrDefault(detail => detail.Contains('€')) ?? "Price not found";
        var price = PriceRegex().Replace(originalPriceDetail, string.Empty);

        // Find the detail that contains a French zip code
        var location = details.FirstOrDefault(detail => ZipCodeRegex().IsMatch(detail)) ?? "Location not found";

        var description = details.FirstOrDefault(detail => detail != originalPriceDetail && detail != location) ??
                          "Description not found";

        var adUrl = node.Attributes.First(attr => attr.Name == "href").Value;
        return new FlatAdDto(location, description, price, adUrl);
    }

    [GeneratedRegex(@"[^0-9,.\u20AC]")]
    private static partial Regex PriceRegex();

    [GeneratedRegex(@"\d{5}")]
    private static partial Regex ZipCodeRegex();
}