using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.Services.SyncDataServices
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;

        public PlatformDataClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Platform>> ReturnAllPlatformsAsync()
        {
            Console.WriteLine($"--> Calling gRPC Service {_configuration["GrpcPlatform"]}");

            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]!);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var response = await client.GetAllPlatformsAsync(request);
                return response.Platform.Select(p => new Platform
                {
                    ExternalId = p.PlatformId,
                    Name = p.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> {nameof(PlatformDataClient)} Could not call gRPC Server: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve platforms from PlatformService via gRPC.", ex);
            }
        }
    }
}
