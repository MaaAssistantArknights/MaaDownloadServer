namespace MaaDownloadServer.Model.General;

public record ResourceInfo(string Path, string RelativePath, string Hash)
{
    public string Path { get; set; } = Path;
    public string RelativePath { get; set; } = RelativePath;
    public string Hash { get; set; } = Hash;
}
