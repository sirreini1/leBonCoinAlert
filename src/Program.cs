using LeBonCoinAlert.core;
using LeBonCoinAlert.DB;

var dbContext = new AppDbContext();
dbContext.Database.EnsureCreated();

var flatAdRepository = new FlatAdRepository(dbContext);
var userPairChatIdRepository = new UserPairChatIdRepository(dbContext);
var telegramHandler = new TelegramHandler(userPairChatIdRepository, flatAdRepository);
var adChecker = new AddChecker(flatAdRepository, telegramHandler);

await adChecker.CheckForNewAdsPeriodically();