using Microsoft.EntityFrameworkCore;
using Semver;

namespace MaaDownloadServer.Services.Controller;

public class DownloadService : IDownloadService
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IComponentService _componentService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public DownloadService(
        IFileSystemService fileSystemService,
        IComponentService componentService,
        MaaDownloadServerDbContext dbContext)
    {
        _fileSystemService = fileSystemService;
        _componentService = componentService;
        _dbContext = dbContext;
    }

    public async Task<PublicContent> GetFullPackage(string componentName, Platform platform, Architecture architecture, SemVersion version)
    {
        var tag = new PublicContentTag(PublicContentTagType.FullPackage, platform, architecture, componentName, version).ParseToTagString();
        var pc = await _dbContext.PublicContents.FirstOrDefaultAsync(x => x.Tag == tag);
        return pc;
    }

    public async Task<PublicContent> GetUpdatePackage(string componentName, Platform platform, Architecture architecture, SemVersion from, SemVersion to)
    {
        var allComponents = await _componentService.GetAllComponents();
        var componentExist = allComponents.Exists(x => x.Name == componentName);
        if (componentExist is false)
        {
            return null;
        }

        if (from == to)
        {
            return null;
        }
        var tag = new PublicContentTag(PublicContentTagType.UpdatePackage, platform, architecture, componentName, from, to).ParseToTagString();
        var pc = await _dbContext.PublicContents.FirstOrDefaultAsync(x => x.Tag == tag);
        var fromVersion = from.ToString();
        var toVersion = to.ToString();
        if (pc is not null)
        {
            return pc;
        }
        var fromPackage = await _dbContext.Packages
            .FirstOrDefaultAsync(x => x.Platform == platform && x.Architecture == architecture && x.Version == fromVersion);
        var toPackage = await _dbContext.Packages
            .FirstOrDefaultAsync(x => x.Platform == platform && x.Architecture == architecture && x.Version == toVersion);
        if (fromPackage is null || toPackage is null)
        {
            return null;
        }
        var diff = _fileSystemService.GetUpdateDiff(fromPackage, toPackage);
        var pcs = await _fileSystemService.AddUpdatePackages(componentName, new List<UpdateDiff> { diff });
        var npc = pcs.First();
        return npc;
    }
}
