// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Modules;

/// <summary>
/// Maa 组件同步状态
/// </summary>
public class MaaSyncStatus : EditableEntity
{
    public MaaSyncStatus(MaaModule maaModule)
    {
        MaaModule = maaModule;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="version">Maa 组件最新版本</param>
    public void Update(MaaVersion version)
    {
        var now = DateTimeOffset.UtcNow;
        LastSync = now;
        LatestVersion = version;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private MaaSyncStatus() { }
#pragma warning restore CS8618

    /// <summary>
    /// Maa 组件
    /// </summary>
    public MaaModule MaaModule { get; private set; }
    /// <summary>
    /// 最后一次同步时间
    /// </summary>
    public DateTimeOffset? LastSync { get; private set; }
    /// <summary>
    /// 最新版本
    /// </summary>
    public MaaVersion? LatestVersion { get; private set; }
}
