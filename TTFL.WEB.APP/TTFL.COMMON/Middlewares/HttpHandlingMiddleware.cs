using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using TTFL.COMMON.Const;
using TTFL.ENTITIES;

namespace TTFL.COMMON.Middlewares
{
    public class HttpHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public HttpHandlingMiddleware(
            RequestDelegate next,
            IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                AppConsts.SelectedSeason = context.Request.Cookies.FirstOrDefault(c => c.Key == "selectedSeason").Value;
                if (!string.IsNullOrEmpty(AppConsts.SelectedSeason))
                {
                    string? path = $"{_config.GetSection("DB_FILES_PATH").Value}{AppConsts.SelectedSeason}.sqlite";
                    ServiceCollection? services = new();
                    services.AddDbContext<TTFLContext>(options =>
                    options.UseSqlite(path)
                    .EnableSensitiveDataLogging());

                    ServiceProvider? serviceProvider = services.BuildServiceProvider();
                    AppConsts.TTFLContext = serviceProvider.GetRequiredService<TTFLContext>();
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
            }
        }

        private static void HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected

            string result = JsonConvert.SerializeObject(new { ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            //return context.Response.WriteAsync(result);
            context.Response.Redirect("../Error");
        }
    }
}
