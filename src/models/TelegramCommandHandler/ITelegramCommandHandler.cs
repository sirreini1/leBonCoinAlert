// src/commands/ITelegramCommandHandler.cs

using Telegram.Bot.Types;

namespace LeBonCoinAlert.models.TelegramCommandHandler;

public interface ITelegramCommandHandler
{
    Task HandleCommand(Message msg, CancellationTokenSource cts);
}