// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Base;

namespace MaaDownloadServer.Data.Base.Entities.Assets;

/// <summary>
/// 资源下载链接
/// </summary>
public class AssetDownloadUrl : BaseEntity
{
    public AssetDownloadUrl(string url, int priority)
    {
        Url = url;
        Priority = priority;
    }

#pragma warning disable disable CS8618
    private AssetDownloadUrl() { }
#pragma warning restore CS8618

    /// <summary>
    /// 资源下载链接
    /// </summary>
    public string Url { get; private set; }
    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; private set; }
}
