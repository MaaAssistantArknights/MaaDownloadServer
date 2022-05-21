using MaaDownloadServer.Data.Base.Entities.Base;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace MaaDownloadServer.Data.Base.Entities.External;

/// <summary>
/// 第三方组件
/// </summary>
public class ExternalModule : BaseEntity
{
    public ExternalModule(string id, string name, string description, string url)
    {
        Id = id;
        Name = name;
        Description = description;
        Url = url;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private ExternalModule() { }
#pragma warning restore CS8618

    /// <summary>
    /// 组件 Id
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 组件简介
    /// </summary>
    public string Description { get; private set; }
    /// <summary>
    /// 组件地址
    /// </summary>
    public string Url { get; private set; }
}
