using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller.GameData;

[ApiController]
[Route("data/stages")]
public class StageController  : ControllerBase {
    private readonly ILogger<StageController> _logger;
    private readonly IConfiguration _configuration;
    private readonly MaaDownloadServerDbContext _dbContext;

    // [HttpOptions]

}
