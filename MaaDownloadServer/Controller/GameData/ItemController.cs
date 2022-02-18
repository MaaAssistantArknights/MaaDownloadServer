using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller.GameData;

[ApiController]
[Route("data/items")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly IConfiguration _configuration;
    private readonly MaaDownloadServerDbContext _dbContext;

    public ItemController(ILogger<ItemController> logger, MaaDownloadServerDbContext dbContext, IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<ArkItem>> GetItemInfo(string name)
    {
        var item = _dbContext.ArkItems.FirstOrDefault(item => item.Name == name);
        if (item is null) {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpGet("{name}/image")]
    [HttpRequestPriority(2)]
    public async Task<ActionResult> GetItemImage(string name)
    {
        var item = _dbContext.ArkItems.FirstOrDefault(item => item.Name == name);
        if (item is null) {
            return NotFound();
        }
        var imagePath = Path.Combine(
            _configuration["MaaServer:DataDirectories:RootPath"],
            _configuration["MaaServer:DataDirectories:SubDirectories:Resources"], 
            $"gamedata/items/{item.Image}"
        );
        if (!System.IO.File.Exists(imagePath)) {
            return NotFound();
        }
        return PhysicalFile(imagePath, "image/png");
    }

    [HttpGet]
    public async Task<ActionResult> ListItemInfo()
    {
        return Ok(_dbContext.ArkItems.ToArray());
    }
};

