using LeBonCoinAlert;
using LeBonCoinAlert.DB;

var dbHandler = new FlatAdRepository();
dbHandler.DeleteAllFlatAds();

// var url = "https://www.leboncoin.fr/recherche?category=10&text=logement&locations=Rennes__48.10980730273463_-1.6674540604783095_7662_5000&price=min-450";
var url =
    "https://www.leboncoin.fr/recherche?category=10&text=logement&locations=Paris__48.86075160420271_2.338756977420986_9301_5000&real_estate_type=2&rooms=2-2&page=3"; //url = UrlConverter.ConvertUrl(url);
var content = await Scraper.FetchPageContent(url);
var adNodes = AdExtractor.GetListingNodes(content);
var flatAdDtos = adNodes.Select(AdExtractor.ExtractAdDetails).ToList(); 
var flatAds = flatAdDtos.Select(dto => new FlatAd(dto, url)).ToList();
dbHandler.UpsertFlatAds(flatAds);

var adsFromDb = dbHandler.ReadFlatAds();

var x = 0;