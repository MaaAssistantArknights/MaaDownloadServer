// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Modules;

/// <summary>
/// Maa 组件版本
/// </summary>
public class MaaVersion : BaseEntity
{
    public MaaVersion(MaaModule maaModule, string version, DateTimeOffset updateTime, string changeLog)
    {
        MaaModule = maaModule;
        Version = version;
        UpdateTime = updateTime;
        ChangeLog = changeLog;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private MaaVersion() { }
#pragma warning restore CS8618

    /// <summary>
    /// Maa 组件
    /// </summary>
    public MaaModule MaaModule { get; private set; }
    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; private set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdateTime { get; private set; }
    /// <summary>
    /// 更新日志
    /// </summary>
    public string ChangeLog { get; private set; }
}
