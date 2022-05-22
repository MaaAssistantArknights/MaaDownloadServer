// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

namespace MaaDownloadServer.Data.Base.Entities.Base;

/// <summary>
/// 可更改实体
/// </summary>
public abstract class EditableEntity : BaseEntity
{
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdateAt { get; protected set; } = DateTimeOffset.UtcNow;
}
