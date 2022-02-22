namespace MaaDownloadServer.Services.Base.Interfaces;

public interface IConfigurationService
{
    string GetPublicDirectory();
    string GetDownloadDirectory();
    string GetResourcesDirectory();
    string GetTempDirectory();
    int GetPublicContentDefaultDuration();
    int GetPublicContentAutoBundledDuration();
}
