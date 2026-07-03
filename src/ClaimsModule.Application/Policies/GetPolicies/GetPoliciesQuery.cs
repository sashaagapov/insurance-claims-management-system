using MediatR;

namespace ClaimsModule.Application.Policies.GetPolicies;

public record GetPoliciesQuery : IRequest<IReadOnlyCollection<PolicyListItemDto>>;
