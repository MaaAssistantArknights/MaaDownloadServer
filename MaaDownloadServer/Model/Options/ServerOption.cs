namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:Server")]
public record ServerOption : IMaaOption
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string ApiFullUrl { get; set; }
}
