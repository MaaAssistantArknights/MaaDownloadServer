using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("announce")]
[ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
public class AnnounceController : ControllerBase
{
    private readonly MaaDownloadServerDbContext _dbContext;

    public AnnounceController(MaaDownloadServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<string> GetAnnounce([FromQuery] string issuer)
    {
        if (string.IsNullOrEmpty(issuer))
        {
            return NotFound();
        }

        var announceCacheObj = _dbContext.DatabaseCaches.FirstOrDefault(x => x.QueryId == $"persist_anno_{issuer}");
        if (announceCacheObj is null)
        {
            return NotFound();
        }

        return announceCacheObj.Value;
    }
}
