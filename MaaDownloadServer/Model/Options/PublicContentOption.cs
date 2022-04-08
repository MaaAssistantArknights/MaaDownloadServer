namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:PublicContent")]
public record PublicContentOption : IMaaOption
{
    public int OutdatedCheckInterval { get; set; }
    public int DefaultDuration { get; set; }
    public int AutoBundledDuration { get; set; }
}
