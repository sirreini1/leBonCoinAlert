﻿using LeBonCoinAlert.DB;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core
{
    public class TelegramHandler(UserPairChatIdRepository userPairChatIdRepository, FlatAdRepository flatAdRepository)
    {
        private readonly TelegramBotClient _bot = CreateBot(flatAdRepository, userPairChatIdRepository);

        public async Task SendMessageToUser(string userId, string message)
        {
            var userChatIdPair = userPairChatIdRepository.FindChatByUserId(userId);
            if (userChatIdPair?.ChatId != null)
            {
                await _bot.SendTextMessageAsync(userChatIdPair.ChatId, message);
            }
            else
            {
                Console.WriteLine("User not found, could not send message");
            }
        }

        public static TelegramBotClient CreateBot(FlatAdRepository flatAdRepository,
            UserPairChatIdRepository userPairChatIdRepository)
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
            Task OnError(Exception exception, HandleErrorSource source)
            {
                Console.WriteLine(exception); // just dump the exception to the console
                return Task.CompletedTask;
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
                if (msg?.Text is null)
                {
                    Console.WriteLine("Error no message");
                    return;
                }

                var msgText = msg.Text.ToLower();

                var telegramUser = msg.From!.Id.ToString();
                var chatId = msg.Chat.Id.ToString();
                userPairChatIdRepository.SaveUserChatIdPair(new UserChatIdPair(telegramUser, chatId));

                if (msgText == "/help")
                {
                    await bot.SendTextMessageAsync(msg.Chat,
                        "Commands:\n /start - Start the bot\n /watch - Watch a search url\n /list - List all watched search urls\n /remove - Remove a search url",
                        cancellationToken: cts.Token,
                        linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                }

                if (msgText == "/start")
                {
                    await bot.SendTextMessageAsync(msg.Chat,
                        "Welcome to the LeBonCoinBot, start watching a search by typing /watch and the search url to watch. Just like this:\n /watch https://www.leboncoin.fr/recherche?.... \n type /help to see all commands",
                        cancellationToken: cts.Token,
                        linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                }

                if (msgText == "/list")
                {
                    var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
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

                if (msgText.StartsWith("/remove"))
                {
                    try
                    {
                        var id = int.Parse(msgText.Split(" ")[1]);
                        var userAds = flatAdRepository.GetFlatAdsForUser(telegramUser);
                        var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct()
                            .OrderByDescending(url => url)
                            .ToList();

                        if (id < 0 || id > uniqueSearchUrls.Count)
                        {
                            await bot.SendTextMessageAsync(msg.Chat, "Invalid id", cancellationToken: cts.Token);
                            return;
                        }

                        var searchUrl = uniqueSearchUrls[id];
                        flatAdRepository.DeleteFlatAdsForUserAndUrl(telegramUser, searchUrl);
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

                if (msgText.StartsWith("/statistics"))
                {
                    var message = flatAdRepository.GetStatisticPerUser(telegramUser);
                    await bot.SendTextMessageAsync(msg.Chat, message, cancellationToken: cts.Token);
                }

                if (msgText.StartsWith("/watch"))
                {
                    var url = msgText.Split(" ")[1];
                    if (!url.StartsWith("https://www.leboncoin.fr/"))
                    {
                        await bot.SendTextMessageAsync(msg.Chat,
                            "Invalid URL. Please provide a valid LeBonCoin search URL.", cancellationToken: cts.Token);
                        return;
                    }

                    var flatAds = await AdExtractor.GetAdsFromUrl(url);
                    if (flatAdRepository.EntriesForURlAndUserExist(url, telegramUser))
                    {
                        await bot.SendTextMessageAsync(msg.Chat, "Already watching ads for this url",
                            cancellationToken: cts.Token,
                            linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
                    }
                    else
                    {
                        flatAdRepository.UpsertFlatAds(flatAds, telegramUser);
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