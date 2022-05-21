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
