// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Other;

namespace MaaDownloadServer.App.Core.Handler.Other;

public class GetAnnounceHandler : IRequestHandler<GetAnnounceCommand, MaaActionResult<GetAnnounceDto>>
{
    private readonly MaaDbContext _dbContext;

    public GetAnnounceHandler(MaaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MaaActionResult<GetAnnounceDto>> Handle(GetAnnounceCommand request, CancellationToken cancellationToken)
    {
        var announce = await _dbContext.Announces
            .FirstOrDefaultAsync(x => x.Publisher == request.Publisher, cancellationToken);
        if (announce is null)
        {
            return MaaApiResponse.NotFound("Announce", request.HttpContext.TraceIdentifier);
        }

        var dto = new MaaDownloadServer.Core.Domain.Models.Other.Announce(announce.UpdateAt.ToStringZhHans(), announce.Publisher, announce.Message);
        return MaaApiResponse.Ok(new GetAnnounceDto(dto), request.HttpContext.TraceIdentifier);
    }
}
