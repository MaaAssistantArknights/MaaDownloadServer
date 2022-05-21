using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Modules;

/// <summary>
/// Maa 组件包
/// </summary>
public class MaaPackage : BaseEntity
{
    public MaaPackage(MaaModule maaModule, MaaVersion maaVersion, Asset asset)
    {
        MaaModule = maaModule;
        MaaVersion = maaVersion;
        Asset = asset;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private MaaPackage() { }
#pragma warning restore CS8618

    /// <summary>
    /// Maa 组件
    /// </summary>
    public MaaModule MaaModule { get; private set; }
    /// <summary>
    /// Maa 组件版本
    /// </summary>
    public MaaVersion MaaVersion { get; private set; }

    /// <summary>
    /// 资源包
    /// </summary>
    public Asset Asset { get; private set; }
}
