using System.ComponentModel.DataAnnotations;

namespace LeBonCoinAlert.DB.entities;

public class UserChatIdPair()
{
    public UserChatIdPair(string userId, string chatId, string id = "") : this()
    {
        UserId = userId;
        ChatId = chatId;
        Id = $"{userId}_{chatId}";
    }

    [MaxLength(100)] public string UserId { get; set; }
    [MaxLength(100)] public string ChatId { get; set; }
    [Key] [MaxLength(200)] public string Id { get; set; }
}