using PuppeteerSharp;

namespace TTFL.COMMON.Helpers.HttpHelper
{
    public class HeadlessHelper
    {
        public static Browser Browser { get; set; }
        public static Page Page { get; set; }

        public static async Task InitBrowserAsync()
        {
#if DEBUG
            string extensionPath = "D:\\DEV\\TTFL\\TTFL.WEB.APP\\TTFL.CONSOLE\\Resources\\2Captcha";
#else
            string extensionPath = "C:\\TTFL\\TTFL_CONSOLE\\Resources\\2Captcha";
#endif

            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Args = new string[] {
                        $"--window-size=1366,768",
                        "--disable-dev-shm-usage",
                        "--lang=fr",
                        $@"--load-extension=""{extensionPath}""",
                        $@"--disable-extensions-except=""{extensionPath}""",
                        "--no-sandbox",
                        "--disable-dev-shm-usage"
                    },
                Timeout = 0,
                SlowMo = 10,
#if DEBUG
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
#else
                ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
#endif
                IgnoreHTTPSErrors = true,

#if DEBUG
                Headless = false
#else
                Headless = false
#endif
            });
            Page = (await Browser.PagesAsync())[0];
        }

        public static async Task GoToAsync(string url)
        {
            await Page.GoToAsync(url);
        }

        public static async Task CloseBrowserAsync()
        {
            if (Browser != null && !Browser.IsClosed)
            {
                await Browser.CloseAsync();
                Browser.Dispose();
            }
        }
    }
}
