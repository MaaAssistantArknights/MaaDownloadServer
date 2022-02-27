namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IArkZoneService
{
    Task<GetZoneDto> GetZone(string zoneId);
    Task<QueryZoneDto> QueryZones(Dictionary<string, string> query);
}
