using System.Text.RegularExpressions;
using LeBonCoinAlert.DB.entities;
using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public partial class StartCommand(
    TelegramBotClient bot,
    IUserPairChatIdRepository userPairChatIdRepository,
    ILogger<TelegramCommand> logger)
    : TelegramCommand(bot, StartRegex(), logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var chatId = msg.Chat.Id.ToString();
        userPairChatIdRepository.SaveUserChatIdPair(new UserChatIdPair(telegramUser, chatId));

        await _bot.SendTextMessageAsync(msg.Chat,
            "Welcome to the LeBonCoinBot, start watching a search by typing /watch and the search url to watch. Just like this:\n /watch https://www.leboncoin.fr/recherche?.... \n type /help to see all commands",
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }

    [GeneratedRegex("/start", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex StartRegex();
}