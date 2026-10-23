namespace PlatformService.Contracts
{
    public record PlatformReadResponse(int Id, string Name, string Publisher, string Cost);

    public record PlatformCreateRequest(string Name, string Publisher, string Cost);
}