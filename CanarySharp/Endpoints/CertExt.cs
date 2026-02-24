using System.Security.Cryptography.X509Certificates;

namespace CanarySharp.Endpoints;

public static class CertExt
{
    public static string GetPathtoCert(string base64Str, string filepath)
    {
        var path = Path.Combine(Path.GetTempPath(), filepath);
        var content = Convert.FromBase64String(base64Str);
        File.WriteAllBytes(path, content);
        return path;
    }

    public static void AddCertsToStore(Dictionary<string, string>[] truststoreCerts)
    {
        using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadWrite);
        var count = 0;
        foreach (var item in truststoreCerts)
        {
            var type = item["Type"].ToLower();
            var value = item["Value"];

            var filepath = type switch
            {
                "filepath" => value,
                "base64" => GetPathtoCert(value, $"truststore-cert-{count++}.pem"),
                _ => throw new Exception("Invalid TruststoreCerts:Type - needs to be filepath or base64"),
            };

            Console.WriteLine($"[info] Truststore added - {filepath}");
            var cert = X509CertificateLoader.LoadCertificateFromFile(filepath);
            store.Add(cert);
        }
        store.Close();
    }
}
