using PlatformService.Contracts;
using System.Text;
using System.Text.Json;

namespace PlatformService.Services.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformCommandAsync(PlatformReadResponse platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json");

            var baseUrl = _configuration["CommandService:BaseUrl"];
            var pingEndpoint = $"{baseUrl}{_configuration["CommandService:Endpoints:Ping"]}";

            var response = await _httpClient.PostAsync(pingEndpoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }
        }
    }
}
