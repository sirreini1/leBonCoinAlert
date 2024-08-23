// src/commands/HelpCommandHandler.cs

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler
{
    public class HelpCommandHandler(TelegramBotClient bot) : ITelegramCommandHandler
    {
        public async Task HandleCommand(Message msg, CancellationTokenSource cts)
        {
            await bot.SendTextMessageAsync(msg.Chat,
                "Commands:\n /start - Start the bot\n /watch - Watch a search url\n /list - List all watched search urls\n /remove - Remove a search url",
                cancellationToken: cts.Token,
                linkPreviewOptions: new LinkPreviewOptions() { IsDisabled = true });
        }
    }
}