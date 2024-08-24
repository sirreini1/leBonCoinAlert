using LeBonCoinAlert.core;
using LeBonCoinAlert.core.LeBonCoin;

namespace LeBonCoinAlert;

public class Application(IAdMonitoringService adMonitoringService)
{
    public async Task StartApp()
    {
        await adMonitoringService.CheckForNewAdsPeriodically();
    }
}