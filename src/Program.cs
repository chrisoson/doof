using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using doof.Data;
using doof.Helpers;
using doof.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;

var builder = WebApplication.CreateBuilder(args);

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

var supportedCultures = builder.Configuration.GetSection("Localization:SupportedCultures").Get<string[]>();
var defaultCulture = builder.Configuration.GetValue<string>("Localization:DefaultCulture");


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture!);
    if (supportedCultures != null)
    {
        options.AddSupportedCultures(supportedCultures);
        options.AddSupportedUICultures(supportedCultures);
    }

    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = options});
});

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
            googleOptions.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"]!;
            googleOptions.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"]!;
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
            facebookOptions.AppId = builder.Configuration["FACEBOOK_APP_ID"]!;
            facebookOptions.AppSecret = builder.Configuration["FACEBOOK_APP_SECRET"]!;
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
            microsoftOptions.ClientId = builder.Configuration["MICROSOFT_CLIENT_ID"]!;
            microsoftOptions.ClientSecret = builder.Configuration["MICROSOFT_CLIENT_SECRET"]!;
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

app.UseRequestCulture(app.Configuration);

app.UseRouting();

app.UseRequestLocalization();

app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/not-found");

app.MapRazorPages();

app.Run();