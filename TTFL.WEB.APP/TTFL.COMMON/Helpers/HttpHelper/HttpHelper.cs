using System.Net;

namespace TTFL.COMMON.Helpers.HttpHelper
{
    public class HttpHelper
    {
        /// <summary>
     /// Get html from url
     /// </summary>
     /// <param name="url"></param>
     /// <returns></returns>
        public static async Task<string> GetHtmlAsync(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using StreamReader GuysReader = new(response.GetResponseStream());
            return await GuysReader.ReadToEndAsync();
        }
    }
}
