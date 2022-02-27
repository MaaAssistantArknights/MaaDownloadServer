namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IArkZoneService
{
    Task<GetZoneDto> GetZone(string zoneId);
    Task<QueryZoneDto> QueryZones(IReadOnlyDictionary<string, string> query);
}
