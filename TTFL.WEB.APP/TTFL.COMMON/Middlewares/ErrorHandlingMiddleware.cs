using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using System.Net;

namespace TTFL.COMMON.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
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
