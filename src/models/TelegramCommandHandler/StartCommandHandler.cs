// src/commands/StartCommandHandler.cs

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler;

public class StartCommandHandler(TelegramBotClient bot) : ITelegramCommandHandler
{
    public async Task HandleCommand(Message msg, CancellationTokenSource cts)
    {
        await bot.SendTextMessageAsync(msg.Chat,
            "Welcome to the LeBonCoinBot, start watching a search by typing /watch and the search url to watch. Just like this:\n /watch https://www.leboncoin.fr/recherche?.... \n type /help to see all commands",
            cancellationToken: cts.Token,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}