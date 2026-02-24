namespace CanarySharp.Endpoints;

public record CallResponse(string Message, Dictionary<string, IEnumerable<string>> Headers, string Data)
{
    public static CallResponse WithMessage(string msg) => new(msg, [], string.Empty);
};
