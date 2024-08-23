using LeBonCoinAlert;
using LeBonCoinAlert.core;
using LeBonCoinAlert.core.LeBonCoinAlert.core;
using Microsoft.Extensions.DependencyInjection;
using LeBonCoinAlert.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection()
    .AddDbContext<AppDbContext>() // Ensure the database is configured correctly
    .AddSingleton<Application>()
    .AddSingleton<IFlatAdRepository, FlatAdRepository>()
    .AddSingleton<IUserPairChatIdRepository, UserPairChatIdRepository>()
    .AddSingleton<ITelegramService, TelegramService>()
    .AddSingleton<ISearchCheckService, SearchCheckService>()
    .AddLogging(configure => { configure.AddConsole(); }) // Add logging
    .BuildServiceProvider();

var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application");

try
{
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    context.Database.Migrate();

    var app = services.GetRequiredService<Application>();
    await app.StartApp();
}
catch (Exception ex)
{

    logger.LogError(ex, "An error occurred while starting the application");
}