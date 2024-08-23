using LeBonCoinAlert.models.TelegramCommandHandler;

namespace LeBonCoinAlert.models.TelegramCommands;

public interface ITelegramCommand
{
    string CommandString { get; }
    ITelegramCommandHandler CommandHandler { get; }
}

public class TelegramCommand(string commandString, ITelegramCommandHandler commandHandler) : ITelegramCommand
{
    public string CommandString { get; } = commandString;
    public ITelegramCommandHandler CommandHandler { get; } = commandHandler;
}