namespace CanarySharp.Endpoints;

public static class DynamicEndpoints
{
    public static WebApplication MapDynamicEndpoints(this WebApplication webapp)
    {
        var ctxPath = webapp.Configuration["ContextPath"] ?? string.Empty;
        var dynGets = webapp.Configuration.GetSection("DynamicGets").Get<string[]>();

        foreach (var path in dynGets ?? [])
        {
            webapp
                .MapGet(UrlExt.Combine(ctxPath, path), () => $"DynamicGet - {path}")
                .WithTags(nameof(DynamicEndpoints));
        }

        return webapp;
    }
}
