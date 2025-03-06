// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System.Net.Http.Json;
using WireMock.Net.Testcontainers;

var container = new WireMockContainerBuilder().WithMappings(Helper.MappingsPath)
                                              .WithAutoRemove(true)
                                              .WithCleanUp(true)
                                              .Build();

await container.StartAsync().ConfigureAwait(false);

var publicBaseUrl = container.GetPublicUrl() + "api";

Console.WriteLine("baseUrl = " + publicBaseUrl);

var wireMockAdminClient = container.CreateWireMockAdminClient();

var settings = await wireMockAdminClient.GetSettingsAsync();
Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

var mappings = await wireMockAdminClient.GetMappingsAsync();
Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

var client = container.CreateClient();

Console.WriteLine("=================================Starting Tests=================================");


var jsonContent = JsonContent.Create(new
{
    batchId = "1234567890",
    transferType = "C2C",
    transactions = new[]
    {
        new
        {
            sourceBankCode = "99",
            countryCode = "NG",
            sourceAccount = "1234567890",
            amountCurrency = "NGN"
        }
    }
});

var onlineJsonWebHookResponse = await client.PostAsync($"{publicBaseUrl}/transaction-online", jsonContent).ConfigureAwait(false);

Console.WriteLine("Online JsonWebHook Response Code = " + onlineJsonWebHookResponse.StatusCode);
Console.WriteLine("Online JsonWebHook Response = " + await onlineJsonWebHookResponse.Content.ReadAsStringAsync());

var localJsonWebHookResponse = await client.PostAsync($"{publicBaseUrl}/transaction-local", jsonContent).ConfigureAwait(false);

Console.WriteLine("Local JsonWebHook Response Code = " + localJsonWebHookResponse.StatusCode);
Console.WriteLine("Local JsonWebHook Response = " + await localJsonWebHookResponse.Content.ReadAsStringAsync());

await container.StopAsync();

Console.WriteLine("Finished Operations");

Console.WriteLine("=================================Finished Tests=================================");

Console.ReadLine();

public static class Helper
{
    private static Lazy<string> RootProjectPath { get; } = new(() =>
    {
        var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
        do
        {
            directoryInfo = directoryInfo.Parent!;
        } while (!directoryInfo.Name.EndsWith("WireMock.NET-WebHook-Demo", StringComparison.OrdinalIgnoreCase));

        return directoryInfo.FullName;
    });

    public static string MappingsPath => Path.Combine(RootProjectPath.Value, "Mappings");
}



