using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeBonCoinAlert.core.telegram.Commands;

public abstract class TelegramCommand
{
    private readonly TelegramBotClient _bot;

    protected TelegramCommand(TelegramBotClient bot, string commandString, ILogger<TelegramCommand> logger)
    {
        _bot = bot;
        CommandString = commandString;
        _bot.OnMessage += async (message, updateType) =>
        {
            if (message.Text!.StartsWith(CommandString, StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    await HandleCommand(message, updateType);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while handling command {CommandString}", CommandString);
                    await bot.SendTextMessageAsync(message.Chat, "An error occurred while processing the command");
                    throw;
                }
            }
        };
    }
    

    private string CommandString { get; }

    protected abstract Task HandleCommand(Message msg, UpdateType updateType);
}