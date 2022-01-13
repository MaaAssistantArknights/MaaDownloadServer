namespace MaaServer.Download.Model.General;

public record PlatformArchCombination(Platform Platform, Architecture Architecture)
{
    public Platform Platform { get; set; } = Platform;
    public Architecture Architecture { get; set; } = Architecture;
}
