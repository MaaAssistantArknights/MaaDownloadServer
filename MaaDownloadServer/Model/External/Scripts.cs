using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public class Scripts
{
    [JsonPropertyName("get_download_info")]
    public string GetDownloadInfo { get; set; }

    [JsonPropertyName("after_download_process")]
    public string AfterDownloadProcess { get; set; }

    [JsonPropertyName("before_add_process")]
    public string BeforeAddProcess { get; set; }

    [JsonPropertyName("relative_path_calculation")]
    public string RelativePathCalculation { get; set; }
}
