namespace CanarySharp.Endpoints;

public static class UrlExt
{
    public static string Combine(params string[] paths)
    {
        return string.Join("/", paths.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim('/')));
    }
}
