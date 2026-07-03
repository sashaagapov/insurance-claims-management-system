using MediatR;

namespace ClaimsModule.Application.Claims.GetClaims;

public record GetClaimsQuery : IRequest<IReadOnlyCollection<ClaimListItemDto>>;
