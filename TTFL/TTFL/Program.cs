using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using TTFL.Services;

namespace TTFL
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static string DbCnx { get; set; }

        public static void Init()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .Build();
        }

        static async Task Main(string[] args)
        {
            Init();
            switch (args[0].ToString())
            {
                case "SR":
                    DbCnx = Configuration.GetSection("ConnectionString")["DefaultConnection"];
                    break;
                case "PO":
                    DbCnx = Configuration.GetSection("ConnectionString")["DefaultConnection_PO"];
                    break;
            }
            await ScrapService.StartScrapAsync();
        }
    }
}
