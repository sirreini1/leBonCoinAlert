using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public abstract class TelegramCommand
{
    private readonly TelegramBotClient _bot;

    protected TelegramCommand(TelegramBotClient bot, string commandString)
    {
        _bot = bot;
        CommandString = commandString;
        _bot.OnMessage += async (message, updateType) =>
        {
            if (message.Text!.StartsWith(CommandString, StringComparison.CurrentCultureIgnoreCase))
                await HandleCommand(message, updateType);
        };
    }

    private string CommandString { get; }

    protected abstract Task HandleCommand(Message msg, UpdateType updateType);
}