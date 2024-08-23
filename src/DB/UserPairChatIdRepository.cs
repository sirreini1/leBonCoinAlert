namespace LeBonCoinAlert.DB;

public interface IUserPairChatIdRepository
{
    void SaveUserChatIdPair(UserChatIdPair? pair);
    UserChatIdPair? FindUserByChatId(string chatId);
    UserChatIdPair? FindChatByUserId(string userId);
}
public class UserPairChatIdRepository(AppDbContext dbContext): IUserPairChatIdRepository
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