using System.Text.Json.Nodes;

namespace CanarySharp.Endpoints;

public record CallRequest(string Url, string Method, JsonObject Data);
