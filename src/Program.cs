using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using doof.Data;
using doof.Helpers;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[] { "en-US", "fr-FR", "es-ES", "sv-SE", "ja-JP" };
var defaultCulture = "en-US";

//If it is in production, i will read env variables in the docker container
//provided by a .env file and passed in the docker-compose.yml file.
var connectionString = builder.Environment.IsProduction()
    ? Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION")!
    : builder.Configuration["SQL_SERVER_CONNECTION"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture(defaultCulture);
    options.AddSupportedUICultures(supportedCultures);
    options.FallBackToParentUICultures = true;
});

builder.Services
    .AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.Add(new CustomCultureRouteModelConvention());
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();


var configuration = builder.Configuration;
//todo set up all the url of these providers when the site is live.
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

//used to redirect the user to the correct culture if the user has not set the culture in the url.
app.Use(async (context, next) =>
{
    var culturePrefix = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    if (!supportedCultures.Contains(culturePrefix))
    {
        var acceptLanguageHeader = context.Request.Headers["Accept-Language"].ToString();
        var preferredCultures = acceptLanguageHeader.Split(',')
            .Select(StringWithQualityHeaderValue.Parse)
            .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
            .Select(s => s.Value)
            .ToList();

        var userCulture = preferredCultures.FirstOrDefault(c => supportedCultures.Contains(c)) ?? defaultCulture;

        if (!context.Request.Path.Value.StartsWith($"/{userCulture}", StringComparison.OrdinalIgnoreCase))
        {
            var redirectPath = $"/{userCulture}{context.Request.Path.Value}";
            context.Response.Redirect(redirectPath);
            return;
        }
    }

    await next();
});
app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();