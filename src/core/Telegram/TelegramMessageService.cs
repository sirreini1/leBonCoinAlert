// src/core/TelegramHandler.cs

using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace LeBonCoinAlert.core.telegram;

public interface ITelegramMessageUserService
{
    Task SendMessageToUser(string userId, string message);
}

public class TelegramMessageUserUserService(
    IUserPairChatIdRepository userPairChatIdRepository,
    TelegramBotClient bot,
    ILogger<TelegramMessageUserUserService> logger)
    : ITelegramMessageUserService
{
    public async Task SendMessageToUser(string userId, string message)
    {
        var userChatIdPair = userPairChatIdRepository.FindChatByUserId(userId);
        if (userChatIdPair?.ChatId != null)
            await bot.SendTextMessageAsync(userChatIdPair.ChatId, message);
        else
            logger.LogError("User not found, could not send message");
    }
}