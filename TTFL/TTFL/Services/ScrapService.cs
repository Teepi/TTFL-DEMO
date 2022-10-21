using HtmlAgilityPack;

using Microsoft.Extensions.Configuration;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TTFL.Helpers.HttpHelper;
using TTFL.Models;
using PuppeteerSharp;
using System.Collections.Generic;
using TTFL.Helpers.DataHelper;
using System.Web;
using TTFL.Helpers.MailHelper;

namespace TTFL.Services
{
    public class ScrapService
    {
        /// <summary>
        /// Scrap Urls ot fetch
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> StartScrapAsync()
        {
            string scrapResult = string.Empty;
            KeyValuePair<int?, bool> pick = await DataHelper.CheckDailyScrapAsync();
            try
            {
                //CHECK DAILY SCRAP

                if (!pick.Value)
                {
                    Console.WriteLine("Init website");
                    IConfigurationSection configUrls = Program.Configuration.GetSection("Urls");
                    IConfigurationSection loginData = Program.Configuration.GetSection("LoginData");
                    await InitWebsite(configUrls, loginData);

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
                scrapResult = $"{ex.Message} : {ex.StackTrace}";
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                await DataHelper.UpdateDailyScrap(pick.Key);

                //Close Browser
                await HeadlessHelper.CloseBrowserAsync();

                await MailHelper.SendMailAsync(scrapResult);
            }
        }

        /// <summary>
        /// Init website Auth
        /// </summary>
        /// <param name="configUrls"></param>
        /// <param name="loginData"></param>
        /// <returns></returns>
        private static async Task InitWebsite(IConfigurationSection configUrls, IConfigurationSection loginData)
        {
            await HeadlessHelper.InitBrowserAsync();
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
                team.Rank = Convert.ToInt32(teamRankNode.InnerText.Trim());
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
                                PickPoints = Convert.ToInt32(Regex.Match(tdList[4].InnerText.Trim(), @"\d+").Value),
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

            return await DataHelper.InsertTeamDataAsync(team);
        }
    }
}
