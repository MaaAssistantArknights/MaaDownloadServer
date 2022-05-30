// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.External;

namespace MaaDownloadServer.App.Core.Handler.External;

public class GetThirdPartyResourceListHandler : IRequestHandler<GetThirdPartyResourceListCommand, MaaActionResult<GetThirdPartyResourceListDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetThirdPartyResourceListHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetThirdPartyResourceListDto>> Handle(GetThirdPartyResourceListCommand request, CancellationToken cancellationToken)
    {
        var externalModuleEntities = await _dbContext.ExternalSyncStatus
            .Include(x => x.ExternalModule)
            .Include(x => x.Asset)
            .OrderBy(x => x.EntityId)
            .Skip((request.Page - 1) * 10)
            .Take(10)
            .ToListAsync(cancellationToken);

        var dto = new GetThirdPartyResourceListDto(externalModuleEntities.Select(Map).ToList());
        return MaaApiResponse.Ok(dto, request.HttpContext.TraceIdentifier);
    }

    private static ExternalModuleInfo Map(ExternalSyncStatus source)
    {
        return new ExternalModuleInfo(
            source.ExternalModule.Id,
            source.ExternalModule.Name,
            source.ExternalModule.Description,
            source.ExternalModule.Url,
            source.LastSync.ToStringZhHans() ?? "",
            source.Asset?.MapAsset());
    }
}
