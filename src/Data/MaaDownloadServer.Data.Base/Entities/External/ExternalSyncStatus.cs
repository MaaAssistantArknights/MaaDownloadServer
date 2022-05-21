using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.External;

/// <summary>
/// 外部组件同步状态
/// </summary>
public class ExternalSyncStatus : EditableEntity
{
    public ExternalSyncStatus(ExternalModule externalModule)
    {
        ExternalModule = externalModule;
    }

    /// <summary>
    /// 更新外部组件
    /// </summary>
    /// <param name="asset">组件资源文件</param>
    public void Update(Asset asset)
    {
        var now = DateTimeOffset.UtcNow;
        LastSync = now;
        Asset = asset;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private ExternalSyncStatus() { }
#pragma warning restore CS8618

    /// <summary>
    /// 外部组件
    /// </summary>
    public ExternalModule ExternalModule { get; private set; }
    /// <summary>
    /// 上次同步时间
    /// </summary>
    public DateTimeOffset? LastSync { get; private set; }
    /// <summary>
    /// 资源文件
    /// </summary>
    public Asset? Asset { get; private set; }
}
