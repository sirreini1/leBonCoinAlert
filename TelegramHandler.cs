using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert
{
    public class TelegramHandler(
        FlatAdRepository flatAdRepository,
        UserPairChatIdRepository userPairChatIdRepository)
    {
        private readonly UserPairChatIdRepository _userPairChatIdRepository = userPairChatIdRepository;
        private readonly FlatAdRepository _flatAdRepository = flatAdRepository;


        public async Task SendMessageToUser(TelegramBotClient bot, string userId,
            string message)
        {
            var userChatIdPair = _userPairChatIdRepository.FindChatByUserId(userId);
            if (userChatIdPair?.ChatId != null)
            {
                await bot.SendTextMessageAsync(userChatIdPair.ChatId, message);
            }
            else
            {
                Console.WriteLine("User not found, could not send message");
            }
        }

        public TelegramBotClient CreateBot()
        {
            var botToken = Environment.GetEnvironmentVariable("TOKEN");
            if (botToken is null)
            {
                throw new Exception("No bot token found in environment variables");
            }

            var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
            bot.OnError += OnError;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;

            Console.WriteLine("Bot started");

            return bot;

            // method to handle errors in polling or in your OnMessage/OnUpdate code
            async Task OnError(Exception exception, HandleErrorSource source)
            {
                Console.WriteLine(exception); // just dump the exception to the console
            }

            // method that handle other types of updates received by the bot:
            async Task OnUpdate(Update update)
            {
                await bot.SendTextMessageAsync(update.Message!.Chat,
                    $"Curently we only support watching ads from leboncoin.fr by typing /watch and the search url",
                    cancellationToken: cts.Token);
            }

            // method that handle messages received by the bot:
            async Task OnMessage(Message? msg, UpdateType type)
            {
                if (msg is null)
                {
                    Console.WriteLine("Error no message");
                    return;
                }

                var telegramUser = msg.From!.Id.ToString();
                var chatId = msg.Chat.Id.ToString();
                _userPairChatIdRepository.SaveUserChatIdPair(new UserChatIdPair(telegramUser, chatId));

                if(msg.Text == "/help")
                {
                    await bot.SendTextMessageAsync(msg.Chat,
                        "Commands:\n /start - Start the bot\n /watch - Watch a search url\n /list - List all watched search urls\n /remove - Remove a search url",
                        cancellationToken: cts.Token,
                        linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                }
                
                if (msg.Text == "/start")
                {
                    await bot.SendTextMessageAsync(msg.Chat,
                        "Welcome to the LeBonCoinBot, start watching a search by typing /watch and the search url to watch. Just like this:\n /watch https://www.leboncoin.fr/recherche?.... \n type /help to see all commands",
                        cancellationToken: cts.Token,
                        linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                }

                if (msg.Text == "/list")
                {
                    var userAds = _flatAdRepository.GetFlatAdsForUser(telegramUser);
                    var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().OrderByDescending(url => url)
                        .ToList();

                    // Format the list of search URLs with their indices
                    var message = "Your watched search URLs:\n";
                    for (int i = 0; i < uniqueSearchUrls.Count; i++)
                    {
                        message += $"Id: {i} URL: {uniqueSearchUrls[i]}\n";
                    }

                    message += $"To remove a url to watch type /remove followed by the id. For example /remove 1";

                    if (uniqueSearchUrls.Count == 0)
                    {
                        message = "You are not watching any search URLs";
                    }

                    // Send the formatted message to the user
                    await bot.SendTextMessageAsync(msg.Chat, message, cancellationToken: cts.Token,
                        linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                }

                if (msg.Text!.StartsWith("/remove"))
                {
                    try
                    {
                        var id = int.Parse(msg.Text.Split(" ")[1]);
                        var userAds = _flatAdRepository.GetFlatAdsForUser(telegramUser);
                        var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct()
                            .OrderByDescending(url => url)
                            .ToList();

                        if (id < 0 || id > uniqueSearchUrls.Count)
                        {
                            await bot.SendTextMessageAsync(msg.Chat, "Invalid id", cancellationToken: cts.Token);
                            return;
                        }

                        var searchUrl = uniqueSearchUrls[id];
                        _flatAdRepository.DeleteFlatAdsForUserAndUrl(telegramUser, searchUrl);
                        await bot.SendTextMessageAsync(msg.Chat, $"Removed search url with id: {id}",
                            cancellationToken: cts.Token);
                    }
                    catch (Exception e)
                    {
                        await bot.SendTextMessageAsync(msg.Chat, "Invalid id", cancellationToken: cts.Token);
                        Console.WriteLine("Error removing search url: " + e.Message);
                        throw;
                    }
                }

                if (msg.Text.StartsWith("/watch"))
                {
                    var url = msg.Text.Split(" ")[1];
                    if (!url.StartsWith("https://www.leboncoin.fr/recherche?"))
                    {
                        await bot.SendTextMessageAsync(msg.Chat,
                            "Invalid URL. Please provide a valid LeBonCoin search URL.", cancellationToken: cts.Token);
                        return;
                    }

                    var flatAds = await AdExtractor.GetAdsFromUrl(url);
                    if (_flatAdRepository.EntriesForURlAndUserExist(url, telegramUser))
                    {
                        await bot.SendTextMessageAsync(msg.Chat, "Already watching ads for this url",
                            cancellationToken: cts.Token,
                            linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                    }
                    else
                    {
                        _flatAdRepository.UpsertFlatAds(flatAds, telegramUser);
                        Console.WriteLine("Starting to watch ads from " + url);
                        await bot.SendTextMessageAsync(msg.Chat, "Starting to watch ads from " + url,
                            cancellationToken: cts.Token,
                            linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                    }
                }
            }
        }
    }
}