namespace CommandService.Contracts
{
    public sealed record PlatformReadDto(int Id, string Name);

    public sealed record CommandReadDto(int Id, string HowTo, string CommandLine, int PlatformId);

    public sealed record CommandCreateDto(string HowTo, string CommandLine);

    public sealed record GenericEventDto(string Event);
}
