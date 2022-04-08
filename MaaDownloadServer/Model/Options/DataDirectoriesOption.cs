namespace MaaDownloadServer.Model.Options;

[ConfigurationSection("MaaServer:DataDirectories")]
public record DataDirectoriesOption : IMaaOption
{
    public string RootPath { get; set; }
    public DataDirectoriesSubDirectoriesOption SubDirectories { get; set; }

    public string Downloads => Path.Combine(RootPath, SubDirectories.Downloads);
    public string Public => Path.Combine(RootPath, SubDirectories.Public);
    public string Resources => Path.Combine(RootPath, SubDirectories.Resources);
    public string Database => Path.Combine(RootPath, SubDirectories.Database);
    public string Temp => Path.Combine(RootPath, SubDirectories.Temp);
    public string Scripts => Path.Combine(RootPath, SubDirectories.Scripts);
    public string Static => Path.Combine(RootPath, SubDirectories.Static);
    public string VirtualEnvironments => Path.Combine(RootPath, SubDirectories.VirtualEnvironments);
}
