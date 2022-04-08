namespace MaaDownloadServer.Model.Options;

public record DataDirectoriesSubDirectoriesOption
{
    public string Downloads { get; set; }
    public string Public { get; set; }
    public string Resources { get; set; }
    public string Database { get; set; }
    public string Temp { get; set; }
    public string Scripts { get; set; }
    public string Static { get; set; }
    public string VirtualEnvironments { get; set; }
}
