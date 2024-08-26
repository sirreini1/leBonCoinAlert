using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public class HelpCommand(TelegramBotClient bot, ILogger<TelegramCommand> logger) : TelegramCommand(bot, "/help", logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        await _bot.SendTextMessageAsync(msg.Chat,
            "Commands:\n /start - Start the bot\n /watch - Watch a search url\n /list - List all watched search urls\n /remove - Remove a search url",
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}