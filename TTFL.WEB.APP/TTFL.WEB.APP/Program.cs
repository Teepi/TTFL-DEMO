
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TTFL.COMMON.Const;
using TTFL.COMMON.Middlewares;
using TTFL.ENTITIES;
using TTFL.SERVICES;
using TTFL.SERVICES.CONTRACT;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

AppConsts.Configuration = configuration;

#if DEBUG
//builder.Services.AddDbContext<TTFLContext>(options =>
//    options.UseSqlite(@"Filename=C:\TFS\Teepi\TTFL\LocalDb\\TTFL_SR_2022_2023.sqlite")
//    .EnableSensitiveDataLogging());

builder.Services.AddDbContext<TTFLContext>(options =>
    options.UseSqlite(@"Filename=D:\\DEV\\TTFL\\LocalDb\\TTFL_SR_2022_2023.sqlite")
    .EnableSensitiveDataLogging());

#else
builder.Services.AddDbContext<TTFLContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CNX_STR"))
    .EnableSensitiveDataLogging());
#endif

ServiceProvider? serviceProvider = builder.Services.BuildServiceProvider();
AppConsts.TTFLContext = serviceProvider.GetRequiredService<TTFLContext>();

builder.Services.AddTransient<IStandingService, StandingService>();
builder.Services.AddTransient<IDateService, DateService>();
builder.Services.AddTransient<IStatsService, StatsService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IPickService, PickService>();
builder.Services.AddTransient<INbaTeamService, NbaTeamService>();

// Add services to the container.
builder.Services.AddRazorPages();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//add userculture provider for authenticated user
RequestLocalizationOptions requestOpt = new();
requestOpt.SupportedCultures = new List<CultureInfo>
        {
            new CultureInfo("fr-FR")
        };
requestOpt.SupportedUICultures = new List<CultureInfo>
        {
            new CultureInfo("fr-FR")
        };
requestOpt.RequestCultureProviders.Clear();
requestOpt.RequestCultureProviders.Add(new SingleCultureProvider());

app.UseRequestLocalization(requestOpt);

app.UseAuthorization();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseMiddleware(typeof(HttpHandlingMiddleware));

app.MapRazorPages();

app.Run();


public class SingleCultureProvider : IRequestCultureProvider
{
    public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        return Task.Run(() => new ProviderCultureResult("fr-FR", "fr-FR"));
    }
}