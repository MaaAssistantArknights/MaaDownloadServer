// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Module;

namespace MaaDownloadServer.App.Core.Handler.Module;

public class GetModuleListHandler : IRequestHandler<GetModuleListCommand, MaaActionResult<GetModuleListDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetModuleListHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetModuleListDto>> Handle(GetModuleListCommand request, CancellationToken cancellationToken)
    {
        var modules = await _dbContext.MaaModules
            .OrderBy(x => x.EntityId)
            .Skip((request.Page - 1) * 10)
            .Take(10)
            .ToListAsync(cancellationToken: cancellationToken);

        var dtos = new List<MaaModuleInfo>();
        foreach (var module in modules)
        {
            var syncStatus = await _dbContext.MaaSyncStatus
                .Include(x => x.MaaModule)
                .Include(x => x.LatestVersion)
                .FirstOrDefaultAsync(x => x.MaaModule.EntityId == module.EntityId, cancellationToken: cancellationToken);

            var syncTime = (syncStatus is null ? null : syncStatus.LastSync.ToStringZhHans()) ?? "";
            var version = syncStatus?.LatestVersion;

            var dto = new MaaModuleInfo(module.Id, module.Name, module.Description, module.Url,
                syncTime,
                version is null
                    ? null
                    : new MaaModuleVersion(version.Version, version.UpdateTime.ToStringZhHans(), version.ChangeLog));
            dtos.Add(dto);
        }

        return MaaApiResponse.Ok(new GetModuleListDto(dtos), request.HttpContext.TraceIdentifier);
    }
}
