using HtmlAgilityPack;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using PuppeteerSharp;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

using TTFL.COMMON.Const;
using TTFL.COMMON.Helpers.HttpHelper;
using TTFL.COMMON.Helpers.MailHelper;
using TTFL.COMMON.Models.Common;
using TTFL.COMMON.Models.Console;
using TTFL.COMMON.Models.Response.NbaTeam;
using TTFL.Services;
using TTFL.SERVICES;

namespace TTFL.CONSOLE
{
    public static class ScrapHelper
    {

        /// <summary>
        /// Scrap Urls ot fetch
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> StartScrapAsync()
        {
            string scrapResult = string.Empty;
            ScrapService scrapService = new();

            //await scrapService.UpdateStatisticsDataAsync();

            KeyValuePair<int?, bool> pick = await scrapService.CheckDailyScrapAsync();
            try
            {
                //CHECK DAILY SCRAP

                if (!pick.Value)
                {
                    Console.WriteLine("Init website");
                    IConfigurationSection configUrls = AppConsts.Configuration.GetSection("Urls");
                    IConfigurationSection loginData = AppConsts.Configuration.GetSection("LoginData");
                    IConfiguration captchaLogin = AppConsts.Configuration.GetSection("2Captcha");
                    await InitWebsite(configUrls, loginData, captchaLogin);

                    //GUYS
                    Console.WriteLine("SCRAP GUYS START");
                    pick = await GetTeamData(configUrls["GUYS_URL"], "Banana Guys");
                    Console.WriteLine("SCRAP GUYS OK");

                    //KIDS
                    Console.WriteLine("SCRAP KIDS START");
                    pick = await GetTeamData(configUrls["KIDS_URL"], "Banana Kids");
                    Console.WriteLine("SCRAP KIDS OK");

                    scrapResult = $"Scrap Finished for {DateTime.Now.Date:dd-MM-yyyy}";
                }
                else
                {
                    scrapResult = $"Scrap ever completed for {DateTime.Now.Date:dd-MM-yyyy}";
                    Console.WriteLine(scrapResult);
                }
                return true;
            }
            catch (Exception ex)
            {
                await scrapService.UpdateDailyScrap(pick.Key, false);
                scrapResult = $"{ex.Message} : {ex.StackTrace}";
                Console.WriteLine(ex?.Message);
                throw;
            }
            finally
            {
                if (pick.Key != 0)
                {
                    await scrapService.UpdateDailyScrap(pick.Key, true);

                    //Close Browser
                    await HeadlessHelper.CloseBrowserAsync();

                    Console.WriteLine("Begin update statistics");
                    await scrapService.UpdateStatisticsDataAsync();
                    Console.WriteLine("End update statistics");

                    await MailHelper.SendMailAsync(scrapResult);
                    Console.WriteLine("End of scrapping");
                }
                else
                {
                    scrapResult = "No scrapping scheduled for today";
                    await MailHelper.SendMailAsync(scrapResult);
                    Console.WriteLine("End of scrapping");
                }
            }
        }

        /// <summary>
        /// Init website Auth
        /// </summary>
        /// <param name="configUrls"></param>
        /// <param name="loginData"></param>
        /// <returns></returns>
        private static async Task InitWebsite(IConfigurationSection configUrls, IConfigurationSection loginData, IConfiguration captchaLogin)
        {
            await HeadlessHelper.InitBrowserAsync();

            Console.WriteLine("Captcha extension login");
            ElementHandle captchaLoginInput = await HeadlessHelper.Page.WaitForXPathAsync("//*[@name='apiKey']");
            if (captchaLoginInput != null)
            {
                await captchaLoginInput.TypeAsync(captchaLogin["API_KEY"]);
            }

            await Task.Delay(2000);

            ElementHandle captchaLoginButton = await HeadlessHelper.Page.WaitForXPathAsync("//*[@id=\"connect\"]");
            if (captchaLoginButton != null)
            {
                await captchaLoginButton.ClickAsync();
            }

            Console.WriteLine("Captcha extension loged");

            await Task.Delay(10000);

            await HeadlessHelper.GoToAsync(configUrls["LOGIN_URL"]);

            await Task.Delay(5000);

            ElementHandle cookieButton = await HeadlessHelper.Page.QuerySelectorAsync("#sd-cmp > div.sd-cmp-2E0Ye > div > div > div > div > div > div > div.sd-cmp-WgGhS.sd-cmp-3YRFa > div.sd-cmp-25TOo > button:nth-child(1)");
            await cookieButton.ClickAsync();

            ElementHandle emailLogin = await HeadlessHelper.Page.WaitForXPathAsync("//input[@name='email'][1]");
            if (emailLogin != null)
            {
                await emailLogin.TypeAsync(loginData["Login"]);
            }

            ElementHandle passwordlLogin = await HeadlessHelper.Page.WaitForXPathAsync("//input[@name='password']");
            if (passwordlLogin != null)
            {
                await passwordlLogin.TypeAsync(loginData["Password"]);
            }

            Console.WriteLine("Trying to resolve Captcha");

            ElementHandle captchaButton = await HeadlessHelper.Page.WaitForXPathAsync("//*[@class='captcha-solver']");
            if (captchaButton != null)
            {
                await captchaButton.ClickAsync();
            }

            HtmlDocument doc = new();
            bool completed = false;
            int retry = 10;
            while (!completed && retry <= 10)
            {
                doc.LoadHtml(await HeadlessHelper.Page.GetContentAsync());
                HtmlNode completedNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"captcha-solver-info\"]");
                if (completedNode != null && completedNode.InnerText.ToLower().Contains("solved"))
                {
                    completed = true;
                    Console.WriteLine("Captcha solved !");
                }
                else
                {
                    Console.WriteLine("Captcha not solved !");
                    await Task.Delay(20000);
                }
            }

            ElementHandle loginButton = await HeadlessHelper.Page.WaitForXPathAsync("//*[@id='contact-form']/div[2]/button");
            if (loginButton != null)
            {
                await loginButton.ClickAsync();
            }
        }

        /// <summary>
        /// Get and insert team data to database
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<KeyValuePair<int?, bool>> GetTeamData(string url, string teamName)
        {
            await Task.Delay(5000);
            await HeadlessHelper.GoToAsync(url);
            await Task.Delay(1000);
            HtmlDocument doc = new();
            doc.LoadHtml(await HeadlessHelper.Page.GetContentAsync());

            TeamResult team = new()
            {
                Players = new List<TeamPlayer>(),
                Name = teamName
            };

            HtmlNode teamRankNode = doc.DocumentNode.SelectSingleNode(".//*[contains(@class,'page-profile')]/div/div/div/div[2]/div/div[2]/strong");
            if (teamRankNode != null)
            {
                team.Rank = Convert.ToInt32(teamRankNode?.InnerText.Trim());
            }

            HtmlNode rankTableNode = doc.DocumentNode.SelectSingleNode("//*[@id='decks']/div/div/table");
            if (rankTableNode != null)
            {
                HtmlNodeCollection rowNodes = rankTableNode.SelectNodes("//*[@id='classementTeamTabme']/tbody/tr");
                if (rowNodes != null)
                {
                    int count = 0;
                    while (count < 10)
                    {
                        count++;
                        HtmlNodeCollection tdList = doc.DocumentNode.SelectNodes($"//*[@id='classementTeamTabme']/tbody/tr[{count}]/td");

                        if (tdList != null)
                        {
                            team.Players.Add(new TeamPlayer
                            {
                                Rank = Convert.ToInt32(tdList[0].InnerText.Trim()),
                                KnickName = HttpUtility.HtmlDecode(tdList[1].InnerText).Trim(),
                                TotalPoints = Convert.ToInt32(tdList[2].InnerText.Trim()),
                                Evolution = Convert.ToInt32(tdList[3].InnerText.Trim()),
                                Pick = HttpUtility.HtmlDecode(tdList[4].InnerText).Trim(),
                                PickPoints = Convert.ToInt32(Regex.Match(tdList[4].InnerText.Trim(), @"[-+]?\d+").Value),
                                BestPick = tdList[4].OuterHtml.Contains("font-weight")
                            });
                        }
                        else
                        {
                            Console.WriteLine("Td List null");
                            throw new Exception("Td List null");
                        }
                    }
                }
            }

            return await new ScrapService().InsertTeamDataAsync(team);
        }

        /// <summary>
        /// Insert or update nba players from file
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InsertOrUpdateNbaPlayerAsync()
        {
            return await new ScrapService().InsertOrUpdateNbaPlayerAsync();
        }

        /// <summary>
        /// Insert or update nba teams from file
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InsertOrUpdateNbaTeamsAsync()
        {
            return await new ScrapService().InsertOrUpdateNbaTeamsAsync();
        }

        /// <summary>
        /// Update calendar of games and dates
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateCalendarAsync()
        {
            string message = string.Empty;
            NbaTeamService nbaTeamService = new();

            try
            {
                Console.WriteLine("Start games dates update");
                string json = await HttpHelper.GetHtmlAsync(AppConsts.Configuration.GetSection("Urls")["NBA_CALENDAR_API_URL"]);
                ScheduleResultJson? games = JsonConvert.DeserializeObject<ScheduleResultJson>(json);

                if (games != null && games?.LeagueSchedule != null)
                {
                    List<string>? dates = games?.LeagueSchedule?.GameDates?
                    .Select(x => DateTime.ParseExact(x.Date.Split(" ").First(), "M/d/yyyy", new CultureInfo("en-US")).ToString("yyyy-M-d"))
                    .Distinct()
                    .ToList();


                    //insert dates
                    if (await new ScrapService().InsertGamesDates(dates))
                    {
                        List<NbaTeamOutput> teams = await nbaTeamService.GetAllNbaTeamsAsync(false);
                        games?.LeagueSchedule?.GameDates?.ForEach(x =>
                        {
                            x?.Games?.ForEach(y =>
                            {
                                NbaTeamOutput? homeTeam = teams.FirstOrDefault(t => t?.TeamSlug == y?.HomeTeam?.TeamName);
                                NbaTeamOutput? awayTeam = teams?.FirstOrDefault(t => t?.TeamSlug == y?.AwayTeam?.TeamName);
                                y.HomeTeam.TeamLogo = !string.IsNullOrEmpty(homeTeam?.Logo) ? homeTeam?.Logo : null;
                                y.HomeTeam.TeamName = !string.IsNullOrEmpty(homeTeam?.TeamName) ? homeTeam?.TeamName : y.HomeTeam.TeamName;
                                y.AwayTeam.TeamLogo = !string.IsNullOrEmpty(awayTeam?.Logo) ? awayTeam?.Logo : null;
                                y.AwayTeam.TeamName = !string.IsNullOrEmpty(awayTeam?.TeamName) ? awayTeam?.TeamName : y?.AwayTeam.TeamName;
                            });
                        });

                        //add file of matches
                        if (File.Exists(AppConsts.GamesFile))
                        {
                            File.Delete(AppConsts.GamesFile);
                        }
                        await File.WriteAllTextAsync(AppConsts.GamesFile, JsonConvert.SerializeObject(games));
                        message = "Date update completed";
                    }
                    else
                    {
                        message = "Error date update";
                    }
                }
                else
                {
                    message = "Error date update";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex?.Message);
                throw;
            }
            finally
            {
                Console.WriteLine(message);
                await MailHelper.SendMailAsync(message);
                Console.WriteLine("End games dates update");
            }

            return true;
        }
    }
}
