using System.Text.Json.Nodes;

namespace CanarySharp.Endpoints;

public record CallRequest(string Url, string Method, RequestOptions Options, JsonObject Data);

public record RequestOptions(string? Host);
