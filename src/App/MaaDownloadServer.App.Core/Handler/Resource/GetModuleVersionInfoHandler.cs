// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Resource;

namespace MaaDownloadServer.App.Core.Handler.Resource;

public class GetModuleVersionInfoHandler : IRequestHandler<GetModuleVersionInfoCommand, MaaActionResult<GetModuleVersionInfoDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetModuleVersionInfoHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetModuleVersionInfoDto>> Handle(GetModuleVersionInfoCommand request, CancellationToken cancellationToken)
    {
        var resource = await _dbContext.MaaPackages
            .Include(x => x.MaaModule)
            .Include(x => x.MaaVersion)
            .Include(x => x.Asset)
            .ThenInclude(x => x.Files)
            .FirstOrDefaultAsync(x => x.MaaModule.Id == request.ModuleId && x.MaaVersion.Version == request.Version, cancellationToken);

        var updatePackages = await _dbContext.MaaUpdatePackages
            .Include(x => x.VersionFrom)
            .Include(x => x.Asset)
            .ThenInclude(x => x.Files)
            .Where(x => x.MaaModule.Id == request.ModuleId)
            .Where(x => x.VersionTo.Version == request.Version)
            .ToListAsync(cancellationToken);

        if (resource is null)
        {
            return MaaApiResponse.NotFound("MaaPackage", request.TraceId);
        }

        var dto = new MaaModuleResource(
            resource.MaaModule.Id, resource.MaaModule.Name, resource.MaaVersion.Version,
            resource.MaaVersion.UpdateTime.ToStringZhHans(), resource.MaaVersion.ChangeLog, Mapper.MapAsset(resource.Asset),
            updatePackages.Select(x => new MaaModuleUpdateResource(
                resource.MaaModule.Id, resource.MaaModule.Name, x.VersionFrom.Version, resource.MaaVersion.Version,
                    new MaaModuleUpdateChanges(
                        x.Update.MapBlobs(),
                        x.Add.MapBlobs(),
                        x.Remove.MapBlobs()),
                    x.Asset.MapAsset())
                ).ToList());

        return MaaApiResponse.Ok(new GetModuleVersionInfoDto(dto), request.TraceId);
    }
}
