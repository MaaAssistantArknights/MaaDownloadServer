// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Module;

namespace MaaDownloadServer.App.Core.Handler.Module;

public class GetModuleVersionListHandler : IRequestHandler<GetModuleVersionListCommand, MaaActionResult<GetModuleVersionListDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetModuleVersionListHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetModuleVersionListDto>> Handle(GetModuleVersionListCommand request, CancellationToken cancellationToken)
    {
        var moduleInfo = await _dbContext.MaaModules
            .FirstOrDefaultAsync(x => x.Id == request.ModuleId, cancellationToken: cancellationToken);
        if (moduleInfo is null)
        {
            return MaaApiResponse.NotFound($"MaaModule {request.ModuleId}", request.HttpContext.TraceIdentifier);
        }

        var maaVersionsQueryable = _dbContext.MaaVersions
            .Include(x => x.MaaModule)
            .Where(x => x.MaaModule.EntityId == moduleInfo.EntityId);

        var maaVersionsCount = await maaVersionsQueryable.CountAsync(cancellationToken: cancellationToken);
        var maaVersions = await maaVersionsQueryable
            .OrderBy(x => x.UpdateTime)
            .Skip((request.Page - 1) * 10)
            .Take(10)
            .ToListAsync(cancellationToken: cancellationToken);

        var dto = new MaaModuleVersionList(moduleInfo.Id, moduleInfo.Name, maaVersionsCount,
            maaVersions.Select(x =>
                    new MaaModuleVersion(x.Version, x.UpdateTime.ToStringZhHans(), x.ChangeLog))
                .ToList());
        return MaaApiResponse.Ok(new GetModuleVersionListDto(dto), request.HttpContext.TraceIdentifier);
    }
}
