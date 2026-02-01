using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Nodes;

namespace CanarySharp.Endpoints;

public static class CanaryEndpoint
{
    public const string XHeaderCanarySent = "X-CanarySharp-Sent";

    public static WebApplication MapCanaryEndpoint(this WebApplication webapp)
    {
        var ctxPath = webapp.Configuration["ContextPath"] ?? string.Empty;
        webapp.MapGet(ctxPath + "version", Version);
        webapp.MapGet(ctxPath + "echo", Echo);
        webapp.MapPost(ctxPath + "call", Call);

        return webapp;
    }

    /// <summary>
    /// Get version of application
    /// </summary>
    public static Ok<string> Version(IConfiguration config)
    {
        return TypedResults.Ok(config["Version"] ?? "no version in config");
    }

    /// <summary>
    /// Return caller's info back - this is for checking what headers are added by loadbalancers/gateways etc
    /// </summary>
    public static Ok<CallResponse> Echo(HttpRequest httpRequest)
    {
        return TypedResults.Ok(
            new CallResponse(
                string.Empty,
                httpRequest.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(x => x ?? string.Empty)),
                string.Empty));
    }

    /// <summary>
    /// Makes a call to remote endpoint
    /// </summary>
    public static async Task<Ok<CallResponse>> Call(
        CallRequest userRequest,
        HttpRequest httpRequest,
        IHttpClientFactory httpFactory)
    {
        // this is to prevent infinite loop when sending to self
        if (httpRequest.Headers.ContainsKey(XHeaderCanarySent))
        {
            return TypedResults.Ok(CallResponse.WithMessage("Already sent once"));
        }

        // send request to remote endpoint
        var client = httpFactory.CreateClient();
        var outgoingMessage = new HttpRequestMessage
        {
            Method = ParseMethod(userRequest.Method),
            RequestUri = new Uri(userRequest.Url),
        };

        // only add content if it's not GET - if not we might get error from remote
        if (outgoingMessage.Method != HttpMethod.Get)
        {
            outgoingMessage.Content = JsonContent.Create(userRequest.Data);
        }

        outgoingMessage.Headers.Add(XHeaderCanarySent, "true");
        var outgoingResponse = await client.SendAsync(outgoingMessage);

        // get response from remote endpoint
        var returnData = await outgoingResponse.Content.ReadAsStringAsync();
        var returnHeaders = outgoingResponse.Headers
                .Concat(outgoingResponse.Content.Headers)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return TypedResults.Ok(
            new CallResponse(
                $"{(int)outgoingResponse.StatusCode} - {outgoingResponse.StatusCode}", 
                returnHeaders, 
                returnData));
    }

    private static HttpMethod ParseMethod(string action) => action.ToUpper() switch
    {
        "GET" => HttpMethod.Get,
        "POST" => HttpMethod.Post,
        _ => throw new InvalidOperationException("Unknown action - needs to be GET/POST")
    };
}

public record CallRequest(string Url, string Method, JsonObject Data);
public record CallResponse(string Message, Dictionary<string, IEnumerable<string>> Headers, string Data)
{
    public static CallResponse WithMessage(string msg) => new(msg, [], string.Empty);
};
