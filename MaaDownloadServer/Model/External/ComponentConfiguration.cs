using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public class ComponentConfiguration
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("metadata_urls")]
    public List<string> MetadataUrl { get; set; }

    [JsonPropertyName("default_url_placeholder")]
    public Dictionary<string, object> UrlPlaceholder { get; set; }

    [JsonPropertyName("after_download_process")]
    public PreProcess<AfterDownloadProcessOperation> AfterDownloadProcess { get; set; }

    [JsonPropertyName("before_add_process")]
    public PreProcess<BeforeAddProcessOperation> BeforeAddProcess { get; set; }

    [JsonPropertyName("scripts")]
    public Scripts Scripts { get; set; }

    [JsonPropertyName("use_proxy")]
    public bool UseProxy { get; set; }

    [JsonPropertyName("pack_update_package")]
    public bool PackUpdatePackage { get; set; }

    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}
