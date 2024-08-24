// src/core/TelegramHandler.cs

using LeBonCoinAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace LeBonCoinAlert.core;

public interface ITelegramMessageService
{
    Task SendMessageToUser(string userId, string message);
}

public class TelegramMessageMessageService(
    IUserPairChatIdRepository userPairChatIdRepository,
    TelegramBotClient bot,
    ILogger<TelegramMessageMessageService> logger)
    : ITelegramMessageService
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