using ClaimsModule.Application.Common;
using MediatR;

namespace ClaimsModule.Application.Reserves.CreateReserve;

public record CreateReserveCommand(
    Guid ClaimId,
    decimal Amount,
    string Currency,
    Guid CreatedByUserId)
    : IRequest<Result<CreateReserveResponse>>;
