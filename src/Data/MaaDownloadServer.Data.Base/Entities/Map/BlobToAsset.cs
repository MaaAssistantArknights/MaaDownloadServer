using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;

namespace MaaDownloadServer.Data.Base.Entities.Map;

public class BlobToAsset : BaseEntity
{
    public Guid BlobId { get; set; }
    public Blob? Blob { get; set; }
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
}
