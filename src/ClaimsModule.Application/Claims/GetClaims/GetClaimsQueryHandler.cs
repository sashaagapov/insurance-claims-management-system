using ClaimsModule.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.GetClaims;

public class GetClaimsQueryHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<GetClaimsQuery, IReadOnlyCollection<ClaimListItemDto>>
{
    public async Task<IReadOnlyCollection<ClaimListItemDto>> Handle(
        GetClaimsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Claims
            .AsNoTracking()
            .OrderByDescending(claim => claim.ReportedAtUtc)
            .Select(claim => new ClaimListItemDto(
                claim.Id,
                claim.ClaimNumber,
                claim.Policy!.PolicyNumber,
                claim.Policy.PolicyholderName,
                claim.CauseOfLossCode!.Code,
                claim.LossDate,
                claim.ReportedAtUtc,
                claim.Status,
                claim.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
