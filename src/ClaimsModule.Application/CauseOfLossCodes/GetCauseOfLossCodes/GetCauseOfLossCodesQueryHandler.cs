using ClaimsModule.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace ClaimsModule.Application.CauseOfLossCodes.GetCauseOfLossCodes;

public class GetCauseOfLossCodesQueryHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<GetCauseOfLossCodesQuery, IReadOnlyCollection<CauseOfLossCodeDto>>
{
    public async Task<IReadOnlyCollection<CauseOfLossCodeDto>> Handle(
        GetCauseOfLossCodesQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.CauseOfLossCodes
            .AsNoTracking()
            .Where(cause => cause.IsActive)
            .OrderBy(cause => cause.Code)
            .Select(cause => new CauseOfLossCodeDto(
                cause.Id,
                cause.Code,
                cause.Description))
            .ToListAsync(cancellationToken);
    }
}
