// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Module;

namespace MaaDownloadServer.App.Core.Handler.Module;

public class GetModuleInfoHandler : IRequestHandler<GetModuleInfoCommand, MaaActionResult<GetModuleInfoDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetModuleInfoHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetModuleInfoDto>> Handle(GetModuleInfoCommand request, CancellationToken cancellationToken)
    {
        var moduleInfo = await _dbContext.MaaModules
            .FirstOrDefaultAsync(x => x.Id == request.ModuleId, cancellationToken: cancellationToken);
        if (moduleInfo is null)
        {
            return MaaApiResponse.NotFound($"MaaModule {request.ModuleId}", request.HttpContext.TraceIdentifier);
        }

        var syncStatus = await _dbContext.MaaSyncStatus
            .Include(x => x.MaaModule)
            .Include(x => x.LatestVersion)
            .FirstOrDefaultAsync(x => x.MaaModule.Id == request.ModuleId, cancellationToken: cancellationToken);

        var syncTime = (syncStatus is null ? null : syncStatus.LastSync.ToStringZhHans()) ?? "";
        var version = syncStatus?.LatestVersion;

        var dto = new MaaModuleInfo(moduleInfo.Id, moduleInfo.Name, moduleInfo.Description, moduleInfo.Url, syncTime,
            version is null ? null
                : new MaaModuleVersion(version.Version, version.UpdateTime.ToStringZhHans(), version.ChangeLog));

        return MaaApiResponse.Ok(new GetModuleInfoDto(dto), request.HttpContext.TraceIdentifier);
    }
}
