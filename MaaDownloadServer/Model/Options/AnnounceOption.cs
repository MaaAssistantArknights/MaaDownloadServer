namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:Announce")]
public record AnnounceOption : IMaaOption
{
    public string[] ServerChanSendKeys { get; set; }
}
