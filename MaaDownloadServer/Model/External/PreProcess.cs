using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public class PreProcess<T> where T : Enum
{
    [JsonPropertyName("operation")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public T Operation { get; set; }

    [JsonPropertyName("args")]
    public Dictionary<string, object> Args { get; set; }
}
