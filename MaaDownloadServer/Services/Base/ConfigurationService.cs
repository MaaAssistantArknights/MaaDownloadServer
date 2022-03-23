namespace MaaDownloadServer.Services.Base;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetPublicDirectory()
    {
        return Path.Combine(
            _configuration["MaaServer:DataDirectories:RootPath"],
            _configuration["MaaServer:DataDirectories:SubDirectories:Public"]);
    }

    public string GetDownloadDirectory()
    {
        return Path.Combine(
            _configuration["MaaServer:DataDirectories:RootPath"],
            _configuration["MaaServer:DataDirectories:SubDirectories:Downloads"]);
    }

    public string GetResourcesDirectory()
    {
        return Path.Combine(
            _configuration["MaaServer:DataDirectories:RootPath"],
            _configuration["MaaServer:DataDirectories:SubDirectories:Resources"]);
    }

    public string GetTempDirectory()
    {
        return Path.Combine(
            _configuration["MaaServer:DataDirectories:RootPath"],
            _configuration["MaaServer:DataDirectories:SubDirectories:Temp"]);
    }

    public int GetPublicContentDefaultDuration()
    {
        return _configuration.GetValue<int>("MaaServer:PublicContent:DefaultDuration");
    }

    public int GetPublicContentAutoBundledDuration()
    {
        return _configuration.GetValue<int>("MaaServer:PublicContent:AutoBundledDuration");
    }
}
