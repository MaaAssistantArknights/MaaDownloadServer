using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Services.Base;

public class AnnounceService : IAnnounceService
{
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly ILogger<AnnounceService> _logger;

    public AnnounceService(MaaDownloadServerDbContext dbContext, ILogger<AnnounceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddAnnounce(string issuer, string message, AnnounceLevel level = AnnounceLevel.Information)
    {
        _logger.LogInformation("添加新的 Announce: [{Level}] {Issuer}: {Message}", level, issuer, message);

        var existAnnounce = await _dbContext.DatabaseCaches.FirstOrDefaultAsync(x => x.QueryId == $"persist_anno_{issuer}");
        if (existAnnounce is not null)
        {
            _dbContext.Remove(existAnnounce);
        }

        var id = Guid.NewGuid();
        var newAnnounce = new Announce(id, DateTime.Now, level, issuer, message);
        var newAnnounceString = JsonSerializer.Serialize(newAnnounce);
        _dbContext.DatabaseCaches.Add(new DatabaseCache
        {
            Id = id,
            QueryId = $"persist_anno_{issuer}",
            Value = newAnnounceString
        });

        await _dbContext.SaveChangesAsync();
    }
}
