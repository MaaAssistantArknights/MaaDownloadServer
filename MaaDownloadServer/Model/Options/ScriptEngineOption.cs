namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:ScriptEngine")]
public record ScriptEngineOption : IMaaOption
{
    public string Python { get; set; }
}
