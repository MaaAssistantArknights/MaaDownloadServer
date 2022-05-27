// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;
using MaaDownloadServer.Core.Domain.Models.Assets;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 更新文件变化
/// </summary>
public class MaaModuleUpdateChanges
{
    public MaaModuleUpdateChanges(ICollection<Blob> update, ICollection<Blob> add, ICollection<Blob> remove)
    {
        Update = update;
        Add = add;
        Remove = remove;
    }

    /// <summary>
    /// 更新文件
    /// </summary>
    [JsonPropertyName("update")]
    public ICollection<Blob> Update { get; }

    /// <summary>
    /// 新增文件
    /// </summary>
    [JsonPropertyName("add")]
    public ICollection<Blob> Add { get; }

    /// <summary>
    /// 移除文件
    /// </summary>
    [JsonPropertyName("remove")]
    public ICollection<Blob> Remove { get; }
}
