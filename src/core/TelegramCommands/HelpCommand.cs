using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.models.TelegramCommands;

public class HelpCommand(TelegramBotClient bot) : TelegramCommand(bot, "/help")
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        await _bot.SendTextMessageAsync(msg.Chat,
            "Commands:\n /start - Start the bot\n /watch - Watch a search url\n /list - List all watched search urls\n /remove - Remove a search url",
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}