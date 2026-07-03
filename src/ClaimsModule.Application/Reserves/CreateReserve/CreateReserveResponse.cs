using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reserves.CreateReserve;

public record CreateReserveResponse(
    Guid ReserveId,
    Guid ClaimId,
    decimal Amount,
    string Currency,
    ReserveStatus Status,
    DateTime CreatedAtUtc);
