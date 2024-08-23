using LeBonCoinAlert.core;
using LeBonCoinAlert.DB;

namespace LeBonCoinAlert;

public class Application(ISearchCheckService searchCheckService)
{
    public async Task StartApp()
    {
        await searchCheckService.CheckForNewAdsPeriodically();
    }
}