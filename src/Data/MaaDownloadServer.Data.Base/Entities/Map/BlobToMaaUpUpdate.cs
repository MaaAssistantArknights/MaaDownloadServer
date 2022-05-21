using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;
using MaaDownloadServer.Data.Base.Entities.Modules;

namespace MaaDownloadServer.Data.Base.Entities.Map;

public class BlobToMaaUpUpdate : BaseEntity
{
    public Guid BlobId { get; set; }
    public Blob? Blob { get; set; }
    public Guid MaaUpdateId { get; set; }
    public MaaUpdatePackage? MaaUpdatePackage { get; set; }
}
