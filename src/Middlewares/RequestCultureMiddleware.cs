using System.Net.Http.Headers;

namespace doof.Middlewares;

public class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string>? _supportedCultures;

    public RequestCultureMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _supportedCultures = configuration.GetSection("Localization:SupportedCultures").Get<List<string>>();
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (context.Request.Method == HttpMethod.Post.Method)
        {
            // If it's a POST request, remove the culture prefix and continue
            var pathSegments = path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathSegments is { Length: > 1 } && _supportedCultures != null && _supportedCultures.Contains(pathSegments[0]))
            {
                // Remove the culture prefix
                context.Request.Path = new PathString("/" + string.Join('/', pathSegments.Skip(1)));
            }

            await _next(context);
            return;
        }

        // Handle GET requests as usual
        var culturePrefix = path?.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        if (_supportedCultures != null && (string.IsNullOrEmpty(culturePrefix) || !_supportedCultures.Contains(culturePrefix)))
        {
            var acceptLanguageHeader = context.Request.Headers["Accept-Language"].ToString();
            var preferredCultures = acceptLanguageHeader.Split(',')
                .Select(StringWithQualityHeaderValue.Parse)
                .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
                .Select(s => s.Value)
                .ToList();

            var userCulture = preferredCultures.FirstOrDefault(c => _supportedCultures.Contains(c)) ?? "en-US";

            if (context.Request.Path.Value != null && !context.Request.Path.Value.StartsWith($"/{userCulture}", StringComparison.OrdinalIgnoreCase))
            {
                var redirectPath = $"/{userCulture}{context.Request.Path.Value}";
                var queryString = context.Request.QueryString.Value;
                context.Response.Redirect(redirectPath + queryString);
                return;
            }
        }

        await _next(context);
    }
}

public static class RequestCultureMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(
        this IApplicationBuilder builder, IConfiguration configuration)
    {
        return builder.UseMiddleware<RequestCultureMiddleware>(configuration);
    }
}