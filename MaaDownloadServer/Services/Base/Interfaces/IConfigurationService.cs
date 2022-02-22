namespace MaaDownloadServer.Services.Base.Interfaces;

public interface IConfigurationService
{
    string GetPublicDirectory();
    string GetDownloadDirectory();
    string GetResourcesDirectory();
    string GetTempDirectory();
    string GetGameDataDirectory();
    int GetPublicContentDefaultDuration();
    int GetPublicContentAutoBundledDuration();
}
