using HtmlAgilityPack;

namespace LeBonCoinAlert;

public class FlatAd(string location, string description, string price)
{
    public string Location { get; set; } = location;
    public string Description { get; set; } = description;
    public string Price { get; set; } = price;
}

public static class AdExtractor
{
    public static List<HtmlNode> GetListingNodes(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var aTagNodes = htmlDoc.DocumentNode.SelectNodes("//a[starts-with(@data-test-id, 'ad')]")
            .Where(node => node.InnerText.Contains("€"))
            .Where(node => !node.InnerHtml.Contains("Sponsorisé"))
            .Where(node => !node.InnerHtml.Contains("advertising"))
            .Where(node => !node.ParentNode.InnerHtml.Contains("Sponsorisé"))
            .Where(node => !node.ParentNode.InnerHtml.Contains("advertising"))
            .ToList();


        return aTagNodes;
    }

    public static FlatAd ExtractAdDetails(HtmlNode node)
    {
        var details = node.SelectNodes(".//p")
            .Select(htmlNode => htmlNode.InnerText).ToArray();

        return new FlatAd(details[2], details[1], details[0]);
    }
}