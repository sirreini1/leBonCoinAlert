namespace LeBonCoinAlert.DB.entities;

public class TelegramUserSearchUrlPair(string telegramUser, string searchUrl)
{
    public string TelegramUser { get; set; } = telegramUser;
    public string SearchUrl { get; set; } = searchUrl;
}