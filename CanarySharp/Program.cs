using CanarySharp.Endpoints;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

if (builder.Configuration.GetSection("HttpsDisableVerify").Get<bool>())
{
    builder.Services.ConfigureHttpClientDefaults(builder =>
    {
        builder.ConfigurePrimaryHttpMessageHandler((handler, provider) =>
        {
            if (handler is HttpClientHandler clientHandler)
            {
                // this disables any ssl checks for server's certificate
                // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.dangerousacceptanyservercertificatevalidator?view=net-10.0
                clientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                Console.WriteLine("[Info] Disable server certificate validation");
            }

            if (handler is SocketsHttpHandler sockets)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.net.security.remotecertificatevalidationcallback?view=net-10.0
                sockets.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            }
        });
    });
}

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(listenOptions =>
    {
        var type = builder.Configuration["HttpsCertP12:Type"];
        var value = builder.Configuration["HttpsCertP12:Value"];

        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("[Info] HttpsCertP12 not defined");
            return;
        }

        var p12File = type.ToLower() switch
        {
            "filepath" => File.Exists(value) ? value : throw new FileNotFoundException($"Cannot find file in HttpsCertP12:Value - {value}"),
            "base64" => CertExt.GetPathtoCert(value, "canary-https.p12"),
            _ => throw new Exception("Invalid HttpsCertP12:Type - needs to be filepath or base64"),
        };

        Console.WriteLine($"[Info] Loading P12 from {p12File}");
        var serverCert = X509CertificateLoader.LoadPkcs12FromFile(p12File, "");
        listenOptions.ServerCertificate = serverCert;
    });
});


// add to truststore
var truststoreCerts = builder.Configuration.GetSection("TruststoreCerts").Get<Dictionary<string, string>[]>() ?? [];
CertExt.AddCertsToStore(truststoreCerts);

var app = builder.Build();
app.MapOpenApi();
app.MapCanaryEndpoint();
app.MapDynamicEndpoints();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/openapi/v1.json", "v1");
});

// disabled to allow swagger in http mode when https is enabled
// app.UseHttpsRedirection();

// not used
// app.UseAuthorization();

app.MapControllers();
app.Run();
