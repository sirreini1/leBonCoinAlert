using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
        if (htmlContent == null)
        {
            throw new ArgumentNullException(nameof(htmlContent), "HTML content cannot be null.");
        }

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
        var price = PriceRegex().Replace(details[0], string.Empty);
        var adUrl = node.Attributes.First(attr => attr.Name == "href").Value;
        return new FlatAdDto(details[2], details[1], price, adUrl);
    }

    [GeneratedRegex(@"[^0-9,.\u20AC]")]
    private static partial Regex PriceRegex();
}