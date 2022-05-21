using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Modules;

/// <summary>
/// Maa 增量更新组件包
/// </summary>
public class MaaUpdatePackage : BaseEntity
{
    public MaaUpdatePackage(MaaModule maaModule, MaaVersion versionTo, MaaVersion versionFrom, Asset asset,
        IReadOnlyList<Blob> update, IReadOnlyList<Blob> add, IReadOnlyList<Blob> remove)
    {
        MaaModule = maaModule;
        VersionTo = versionTo;
        VersionFrom = versionFrom;
        Update = update;
        Add = add;
        Remove = remove;
        Asset = asset;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private MaaUpdatePackage() { }
#pragma warning restore CS8618

    /// <summary>
    /// Maa 组件
    /// </summary>
    public MaaModule MaaModule { get; private set; }
    /// <summary>
    /// 目标版本
    /// </summary>
    public MaaVersion VersionTo { get; private set; }
    /// <summary>
    /// 起始版本
    /// </summary>
    public MaaVersion VersionFrom { get; private set; }

    /// <summary>
    /// 资源包
    /// </summary>
    public Asset Asset { get; private set; }

    /// <summary>
    /// 更新文件
    /// </summary>
    public IReadOnlyList<Blob> Update { get; private set; }
    /// <summary>
    /// 新增文件
    /// </summary>
    public IReadOnlyList<Blob> Add { get; private set; }
    /// <summary>
    /// 删除文件
    /// </summary>
    public IReadOnlyList<Blob> Remove { get; private set; }
}
