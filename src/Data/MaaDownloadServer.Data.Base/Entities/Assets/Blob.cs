using MaaDownloadServer.Data.Base.Entities.Base;
using MaaDownloadServer.Data.Base.Entities.Modules;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.Assets;

/// <summary>
/// 文件块
/// </summary>
public class Blob : EditableEntity
{
    public Blob(string name, string sha1, string md5, string path)
    {
        Name = name;
        Sha1 = sha1;
        Md5 = md5;
        Path = path;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private Blob() { }
#pragma warning restore CS8618

    /// <summary>
    /// 文件名
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 文件 Sha1 校验
    /// </summary>
    public string Sha1 { get; private set; }
    /// <summary>
    /// 文件 Md5 校验
    /// </summary>
    public string Md5 { get; private set; }
    /// <summary>
    /// 文件相对路径
    /// </summary>
    public string Path { get; private set; }

    #region 多对多

    public IReadOnlyCollection<MaaUpdatePackage>? MaaUpAdd { get; set; }
    public IReadOnlyCollection<MaaUpdatePackage>? MaaUpRemove { get; set; }
    public IReadOnlyCollection<MaaUpdatePackage>? MaaUpUpdate { get; set; }
    public IReadOnlyCollection<Asset>? Assets { get; set; }

    #endregion
}
