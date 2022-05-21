using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Assets;

/// <summary>
/// 资源包
/// </summary>
public class Asset : EditableEntity
{
    public Asset(Blob blob, string url, bool isBundle, IReadOnlyList<Blob> blobs)
    {
        Blob = blob;
        Url = url;
        IsBundle = isBundle;
        Files = blobs;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private Asset() { }
#pragma warning restore CS8618

    /// <summary>
    /// 新增一次下载
    /// </summary>
    /// <returns></returns>
    public int AddDownloadCount()
    {
        DownloadCount++;
        return DownloadCount;
    }

    /// <summary>
    /// 资源对应文件
    /// </summary>
    public Blob Blob { get; private set; }
    /// <summary>
    /// 外部访问 URL
    /// </summary>
    public string Url { get; private set; }
    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; private set; }
    /// <summary>
    /// 是组合包
    /// </summary>
    public bool IsBundle { get; private set; }
    /// <summary>
    /// 组合包资源所包含文件
    /// </summary>
    public IReadOnlyList<Blob> Files { get; private set; }
}
