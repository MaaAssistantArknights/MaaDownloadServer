namespace MaaDownloadServer.Services.IServices;

public interface IConfigurationService
{
    string GetPublicDirectory();
    string GetDownloadDirectory();
    string GetResourcesDirectory();
    int GetPublicContentDefaultDuration();
    int GetPublicContentAutoBundledDuration();
}
