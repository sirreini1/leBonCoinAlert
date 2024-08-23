namespace LeBonCoinAlert.models;

public interface ITelegramCommand
{
    string CommandString { get; set; }
    Func<Task> CommandFunction { get; set; }
}

public class TelegramCommand(string commandString, Func<Task> commandFunction) : ITelegramCommand
{
    public string CommandString { get; set; } = commandString;
    public Func<Task> CommandFunction { get; set; } = commandFunction;
}