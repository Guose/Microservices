using Grpc.Core;
using PlatformService.Data.Repos;

namespace PlatformService.Services.SyncDataServices.Gprc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _platformRepo;

        public GrpcPlatformService(IPlatformRepo platformRepo)
        {
            _platformRepo = platformRepo;
        }

        public override async Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = await _platformRepo.GetAllPlatformsAsync();
            foreach (var platform in platforms)
            {
                response.Platform.Add(new GrpcPlatformModel
                {
                    PlatformId = platform.Id,
                    Name = platform.Name,
                    Publisher = platform.Publisher,
                });
            }
            return response;
        }
    }
}
