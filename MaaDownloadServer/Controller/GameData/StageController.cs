using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller.GameData;

[ApiController]
[Route("data/stages")]
public class StageController : ControllerBase
{
    private readonly ILogger<StageController> _logger;
    private readonly MaaDownloadServerDbContext _dbContext;

    public StageController(ILogger<StageController> logger, MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<ArkStage>> GetStageInfo(string name)
    {
        var stage = _dbContext.ArkStages.FirstOrDefault(stage => stage.StageName == name);
        if (stage is null)
        {
            return NotFound();
        }
        return Ok(stage);
    }

    [HttpGet]
    public async Task<ActionResult> ListStagesInfo()
    {
        return Ok(_dbContext.ArkStages.ToArray());
    }
}
