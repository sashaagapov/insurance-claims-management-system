using MediatR;

namespace ClaimsModule.Application.Claims.GetClaimById;

public record GetClaimByIdQuery(Guid ClaimId) : IRequest<ClaimDetailDto?>;
