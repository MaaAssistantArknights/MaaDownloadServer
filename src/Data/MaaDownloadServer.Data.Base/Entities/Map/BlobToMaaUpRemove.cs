// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;
using MaaDownloadServer.Data.Base.Entities.Modules;

namespace MaaDownloadServer.Data.Base.Entities.Map;

public class BlobToMaaUpRemove : BaseEntity
{
    public Guid BlobId { get; set; }
    public Blob? Blob { get; set; }
    public Guid MaaUpdateId { get; set; }
    public MaaUpdatePackage? MaaUpdatePackage { get; set; }
}
