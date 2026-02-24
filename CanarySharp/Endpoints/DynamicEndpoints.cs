namespace CanarySharp.Endpoints;

public static class DynamicEndpoints
{
    public static WebApplication MapDynamicEndpoints(this WebApplication webapp)
    {
        var ctxPath = webapp.Configuration["ContextPath"] ?? string.Empty;
        var dynGets = webapp.Configuration.GetSection("DynamicGets").Get<string[]>();
        var addCtxPath = webapp.Configuration.GetSection("DynamicAppendCtxPath").Get<bool>();

        foreach (var path in dynGets ?? [])
        {
            var apipath = addCtxPath ? UrlExt.Combine(ctxPath, path) : path;
            webapp
                .MapGet(apipath, () => $"DynamicGet - {path}")
                .WithTags(nameof(DynamicEndpoints));
        }

        return webapp;
    }
}
