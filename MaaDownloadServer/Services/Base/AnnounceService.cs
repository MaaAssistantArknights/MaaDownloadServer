using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaaDownloadServer.Services.Base;

public class AnnounceService : IAnnounceService
{
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AnnounceOption> _announceOption;
    private readonly ILogger<AnnounceService> _logger;

    public AnnounceService(MaaDownloadServerDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IOptions<AnnounceOption> announceOption,
        ILogger<AnnounceService> logger)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _announceOption = announceOption;
        _logger = logger;
    }

    public async Task AddAnnounce(string issuer, string title, string message, AnnounceLevel level = AnnounceLevel.Information)
    {
        _logger.LogInformation("加入新的 Announce: [{Level}] {Issuer}: {Message}", level, issuer, message);

        #region Database

        var existAnnounce =
            await _dbContext.DatabaseCaches.FirstOrDefaultAsync(x => x.QueryId == $"persist_anno_{issuer}");
        if (existAnnounce is not null)
        {
            _dbContext.Remove(existAnnounce);
        }

        var plainApiMessageString = $"{title} - {message}";
        var id = Guid.NewGuid();
        var newAnnounce = new Announce(id, DateTime.Now, level, issuer, plainApiMessageString);
        var newAnnounceString = JsonSerializer.Serialize(newAnnounce);
        _dbContext.DatabaseCaches.Add(new DatabaseCache
        {
            Id = id,
            QueryId = $"persist_anno_{issuer}",
            Value = newAnnounceString
        });

        await _dbContext.SaveChangesAsync();

        #endregion

        #region Server Chan

        if (_announceOption.Value.ServerChanSendKeys.Length == 0)
        {
            return;
        }

        var levelShort = level switch
        {
            AnnounceLevel.Information => "INF",
            AnnounceLevel.Warning => "WRN",
            AnnounceLevel.Error => "ERR",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        var serverChanMessageContent = $"[{levelShort}] {message}";
        var form = new List<KeyValuePair<string, string>>
            {
                new("title", title),
                new("desp", serverChanMessageContent)
            };
        var serverChanHttpClient = _httpClientFactory.CreateClient("ServerChan");

        foreach (var key in _announceOption.Value.ServerChanSendKeys)
        {
            var responseMessage = await serverChanHttpClient.PostAsync($"{key}.send", new FormUrlEncodedContent(form));
            var body = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode is false)
            {
                _logger.LogError("推送消息至 ServerChan 失败，状态码：{ServerChanStatusCode}，消息体：{ServerChanContent}",
                    responseMessage.StatusCode, body);
            }
        }

        #endregion
    }
}
