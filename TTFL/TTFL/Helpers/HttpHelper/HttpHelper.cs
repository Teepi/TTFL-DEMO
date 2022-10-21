using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TTFL.Helpers.HttpHelper
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
