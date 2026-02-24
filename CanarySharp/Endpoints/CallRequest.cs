using System.ComponentModel;
using System.Text.Json.Nodes;

namespace CanarySharp.Endpoints;

public record CallRequest(
    [property: DefaultValue("http://localhost:8080/api/version")] string Url,
    [property: DefaultValue("get")] string Method, 
    [property: DefaultValue("localhost")] string? Host, 
    JsonObject Data);
