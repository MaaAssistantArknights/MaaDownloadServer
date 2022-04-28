namespace MaaDownloadServer.Services.Base.Interfaces;

public interface IAnnounceService
{
    Task AddAnnounce(string issuer, string title, string message, AnnounceLevel level = AnnounceLevel.Information);
}
