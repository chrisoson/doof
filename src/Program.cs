using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using doof.Data;
using doof.Helpers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;


var builder = WebApplication.CreateBuilder(args);

var supportedAppLanguages = builder.Configuration.GetSection("SupportedAppLanguages").Get<SupportedAppLanguages>();
var supportedCultures = supportedAppLanguages.Dict.Values.Select(langInApp => langInApp.Culture).ToArray();

//If it is in production, i will read env variables in the docker container
//provided by a .env file and passed in the docker-compose.yml file.
var connectionString = builder.Environment.IsProduction()
    ? Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION")!
    : builder.Configuration["SQL_SERVER_CONNECTION"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddRazorPages()
    .AddRazorPagesOptions(o =>
    {
        o.Conventions.Add(new CustomCultureRouteRouteModelConvention());
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = options});
});

builder.Services.Configure<SupportedAppLanguages>(builder.Configuration.GetSection("AppLanguages"));

var configuration = builder.Configuration;

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        if (builder.Environment.IsProduction())
        {
            googleOptions.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!;
            googleOptions.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!;
        }
        else
        {
            googleOptions.ClientId = configuration["GOOGLE_CLIENT_ID"]!;
            googleOptions.ClientSecret = configuration["GOOGLE_CLIENT_SECRET"]!;
        }
    })
    //This is not working right now, set this up when the website is live.
    .AddFacebook(facebookOptions =>
    {
        if (builder.Environment.IsProduction())
        {
            facebookOptions.AppId = Environment.GetEnvironmentVariable("FACEBOOK_APP_ID")!;
            facebookOptions.AppSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET")!;
        }
        else
        {
            facebookOptions.AppId = configuration["FACEBOOK_APP_ID"]!;
            facebookOptions.AppSecret = configuration["FACEBOOK_APP_SECRET"]!;
        }
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        if (builder.Environment.IsProduction())
        {
            microsoftOptions.ClientId = Environment.GetEnvironmentVariable("MICROSOFT_CLIENT_ID")!;
            microsoftOptions.ClientSecret = Environment.GetEnvironmentVariable("MICROSOFT_CLIENT_SECRET")!;
        }
        else
        {
            microsoftOptions.ClientId = configuration["MICROSOFT_CLIENT_ID"]!;
            microsoftOptions.ClientSecret = configuration["MICROSOFT_CLIENT_SECRET"]!;
        }
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var culturePrefix = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    if (string.IsNullOrEmpty(culturePrefix) || !supportedCultures.Contains(culturePrefix))
    {
        var acceptLanguageHeader = context.Request.Headers["Accept-Language"].ToString();
        var preferredCultures = acceptLanguageHeader.Split(',')
            .Select(StringWithQualityHeaderValue.Parse)
            .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
            .Select(s => s.Value)
            .ToList();

        var userCulture = preferredCultures.FirstOrDefault(c => supportedCultures.Contains(c)) ?? "en-US";

        if (!context.Request.Path.Value.StartsWith($"/{userCulture}", StringComparison.OrdinalIgnoreCase))
        {
            var redirectPath = $"/{userCulture}{context.Request.Path.Value}";
            var queryString = context.Request.QueryString.Value;
            context.Response.Redirect(redirectPath + queryString);
            return;
        }
    }

    await next();
});

app.UseRouting();

app.UseRequestLocalization();

app.UseAuthorization();
app.UseStatusCodePagesWithRedirects("/error/{0}");


app.MapRazorPages();

app.Run();