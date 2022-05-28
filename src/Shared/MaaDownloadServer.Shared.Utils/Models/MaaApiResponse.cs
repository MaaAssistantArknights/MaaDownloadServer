// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Shared.Utils.Models;

public class MaaApiResponse
{
    private MaaApiResponse(int statusCode, string message, string traceId, object? data)
    {
        StatusCode = statusCode;
        Message = message;
        TraceId = traceId;
        Data = data;
    }

    public static MaaApiResponse Ok(object? obj, string id)
    {
        return new MaaApiResponse(200, "OK", id, obj);
    }

    public static MaaApiResponse NotFound(string resourceName, string id)
    {
        return new MaaApiResponse(404, $"{resourceName} Not Found", id, null);
    }

    public static MaaApiResponse InternalError(string id)
    {
        return new MaaApiResponse(500, "Internal Server Error", id, null);
    }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; }
    [JsonPropertyName("message")]
    public string Message { get; }
    [JsonPropertyName("trace_id")]
    public string TraceId { get; }
    [JsonPropertyName("data")]
    public object? Data { get; }
}
