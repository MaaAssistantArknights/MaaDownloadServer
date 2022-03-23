using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("list")]
public class ListController : ControllerBase
{
    private readonly DirectoryInfo _staticDirectory;

    public ListController(IConfiguration configuration)
    {
        _staticDirectory = new DirectoryInfo(Path.Combine(configuration["MaaServer:DataDirectories:RootPath"],
            configuration["MaaServer:DataDirectories:SubDirectories:Static"]));
    }

    [HttpGet("static")]
    public ActionResult<List<string>> GetStaticFileList()
    {
        var files = _staticDirectory.GetFiles("*", SearchOption.AllDirectories);
        var rPaths = files.Select(x => x.FullName.Replace(_staticDirectory.FullName, ""));
        return Ok(rPaths);
    }
}
