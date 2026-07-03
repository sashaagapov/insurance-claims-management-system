using ClaimsModule.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace ClaimsModule.Application.Policies.GetPolicies;

public class GetPoliciesQueryHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<GetPoliciesQuery, IReadOnlyCollection<PolicyListItemDto>>
{
    public async Task<IReadOnlyCollection<PolicyListItemDto>> Handle(
        GetPoliciesQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Policies
            .AsNoTracking()
            .OrderBy(policy => policy.PolicyNumber)
            .Select(policy => new PolicyListItemDto(
                policy.Id,
                policy.PolicyNumber,
                policy.PolicyholderName,
                policy.ProductType,
                policy.EffectiveFrom,
                policy.EffectiveTo,
                policy.Status,
                policy.CoverageLimit,
                policy.Currency))
            .ToListAsync(cancellationToken);
    }
}
