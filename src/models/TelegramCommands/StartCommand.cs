using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.models.TelegramCommands;

public class StartCommand(TelegramBotClient bot) : TelegramCommand(bot, "/start")
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        await _bot.SendTextMessageAsync(msg.Chat,
            "Welcome to the LeBonCoinBot, start watching a search by typing /watch and the search url to watch. Just like this:\n /watch https://www.leboncoin.fr/recherche?.... \n type /help to see all commands",
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}