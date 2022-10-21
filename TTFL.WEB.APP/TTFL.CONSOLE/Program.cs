using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TTFL.COMMON.Const;
using TTFL.CONSOLE;
using TTFL.ENTITIES;



Init();
switch (args[0].ToString())
{
    case "SR":
        AppConsts.DbCnx = AppConsts.Configuration.GetSection("ConnectionString")["DefaultConnection"];
        break;
    case "PO":
        AppConsts.DbCnx = AppConsts.Configuration.GetSection("ConnectionString")["DefaultConnection_PO"];
        break;
    case "NBA_PLAYERS":
        AppConsts.NbaPlayersFile = AppConsts.Configuration.GetSection("Files")["PLAYERS_FILE"];
        break;
    case "NBA_TEAMS":
        AppConsts.NbaTeamsFile = AppConsts.Configuration.GetSection("Files")["TEAMS_FILE"];
        break;
    case "UPDATE_CALENDAR":
#if DEBUG
        AppConsts.DbCnx = "Filename=D:\\DEV\\TTFL\\LocalDb\\TTFL_SR_2022_2023.sqlite";
#else
        AppConsts.DbCnx = AppConsts.Configuration.GetSection("ConnectionString")["DefaultConnection"];
#endif
        AppConsts.GamesFile = AppConsts.Configuration.GetSection("Files")["GAMES_FILE"];
        break;
}

Console.WriteLine(AppConsts.DbCnx);
ServiceCollection? services = new();

#if DEBUG
services.AddDbContext<TTFLContext>(options =>
    options.UseSqlite(@"Filename=D:\DEV\TTFL\LocalDb\TTFL_SR_2022_2023.sqlite")
    .EnableSensitiveDataLogging());

//services.AddDbContext<TTFLContext>(options =>
//    options.UseSqlite("Filename=C:\\TFS\\Teepi\\TTFL\\LocalDb\\TTFL_SR_2022_2023.sqlite")
//    .EnableSensitiveDataLogging());
#else
services.AddDbContext<TTFLContext>(options =>
    options.UseSqlite(AppConsts.DbCnx)
    .EnableSensitiveDataLogging());
#endif

ServiceProvider? serviceProvider = services.BuildServiceProvider();
AppConsts.TTFLContext = serviceProvider.GetRequiredService<TTFLContext>();

switch (args[0])
{
    case "NBA_PLAYERS":
        await ScrapHelper.InsertOrUpdateNbaPlayerAsync();
        break;
    case "NBA_TEAMS":
        await ScrapHelper.InsertOrUpdateNbaTeamsAsync();
        break;
    case "SR" or "PO":
        await ScrapHelper.StartScrapAsync();
        break;
    case "UPDATE_CALENDAR":
        await ScrapHelper.UpdateCalendarAsync();
        break;
}


static void Init()
{
    AppConsts.Configuration = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json")
                          .Build();
}



