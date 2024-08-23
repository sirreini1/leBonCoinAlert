using LeBonCoinAlert.core;

namespace LeBonCoinAlert;

public class Application(ISearchCheckService searchCheckService)
{
    public async Task StartApp()
    {
        await searchCheckService.CheckForNewAdsPeriodically();
    }
}