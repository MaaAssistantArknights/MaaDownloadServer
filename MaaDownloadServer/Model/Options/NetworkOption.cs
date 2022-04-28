namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:Network")]
public record NetworkOption : IMaaOption
{
    public string Proxy { get; set; }
    public string UserAgent { get; set; }
}
