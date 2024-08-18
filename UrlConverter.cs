using System.Web;

namespace LeBonCoinAlert;

public static class UrlConverter
{
    public static string ConvertUrl(string originalUrl)
    {
        // Parse the original URL
        // Parse the original URL
        var uri = new Uri(originalUrl);
        var query = HttpUtility.ParseQueryString(uri.Query);

        // Check if the "page" parameter is present
        if (string.IsNullOrEmpty(query["page"]))
            // Add "page=1" if not present
            query["page"] = "1";

        // Build the new URL
        var baseUrl = "https://www.leboncoin.fr/_next/data/j4OSc3Ywp1mq_t0PFW85K/recherche.json";
        var newQuery = query.ToString();
        var newUrl = $"{baseUrl}?{newQuery}";

        return newUrl;
    }
}