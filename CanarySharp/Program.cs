using CanarySharp.Endpoints;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
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
            "base64" => GetP12FilePath(value),
            _ => throw new Exception("Invalid HttpsCertP12:Type - needs to be filepath or base64"),
        };

        static string GetP12FilePath(string value)
        {
            var path = Path.Combine(Path.GetTempPath(), "canary-https.p12");
            var content = Convert.FromBase64String(value);
            File.WriteAllBytes(path, content);
            return path;
        }

        Console.WriteLine($"[Info] Loading P12 from {p12File}");
        var serverCert = X509CertificateLoader.LoadPkcs12FromFile(p12File, "");
        listenOptions.ServerCertificate = serverCert;
    });
});

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
