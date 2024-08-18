using System.ComponentModel.DataAnnotations;

namespace LeBonCoinAlert.DB;

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

public class UserPairChatIdRepository(AppDbContext dbContext)
{
    public void SaveUserChatIdPair(UserChatIdPair? pair)
    {
        var existingPair = dbContext.UserChatIdPairs.Find(pair.Id);
        if (existingPair != null)
        {
            dbContext.Entry(existingPair).CurrentValues.SetValues(pair);
        }
        else
        {
            dbContext.UserChatIdPairs.Add(pair);
        }

        dbContext.SaveChanges();
    }

    public UserChatIdPair? FindUserByChatId(string chatId)
    {
        return dbContext.UserChatIdPairs.FirstOrDefault(pair => pair != null && pair.ChatId == chatId);
    }

    public UserChatIdPair? FindChatByUserId(string userId)
    {
        return dbContext.UserChatIdPairs.FirstOrDefault(pair => pair != null && pair.UserId == userId);
    }
}