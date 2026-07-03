using System.Globalization;
using ClaimsModule.Application.Common;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.RejectReserve;

public class RejectReserveCommandHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<RejectReserveCommand, Result<RejectReserveResponse>>
{
    public async Task<Result<RejectReserveResponse>> Handle(
        RejectReserveCommand request,
        CancellationToken cancellationToken)
    {
        var claimExists = await dbContext.Claims
            .AsNoTracking()
            .AnyAsync(claim => claim.Id == request.ClaimId, cancellationToken);

        if (!claimExists)
        {
            return Result<RejectReserveResponse>.Failure("NOT_FOUND: Claim was not found.");
        }

        var reserve = await dbContext.Reserves
            .FirstOrDefaultAsync(
                item => item.Id == request.ReserveId && item.ClaimId == request.ClaimId,
                cancellationToken);

        if (reserve is null)
        {
            return Result<RejectReserveResponse>.Failure("NOT_FOUND: Reserve was not found for this claim.");
        }

        var actor = await dbContext.MockUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Id == request.ActorUserId && user.IsActive,
                cancellationToken);

        if (actor is null)
        {
            return Result<RejectReserveResponse>.Failure("BAD_REQUEST: Actor user was not found or is not active.");
        }

        if (reserve.CreatedByUserId == request.ActorUserId)
        {
            return Result<RejectReserveResponse>.Failure("UNPROCESSABLE: Users cannot reject their own reserve.");
        }

        if (actor.Role is not UserRole.Supervisor and not UserRole.Manager)
        {
            return Result<RejectReserveResponse>.Failure("FORBIDDEN: Only a supervisor or manager can reject reserves.");
        }

        if (reserve.Status != ReserveStatus.PendingApproval)
        {
            return Result<RejectReserveResponse>.Failure(
                $"UNPROCESSABLE: Only PendingApproval reserves can be rejected. Current status is {reserve.Status}.");
        }

        var rejectedAtUtc = DateTime.UtcNow;
        var reason = request.Reason.Trim();
        var formattedAmount = reserve.Amount.ToString("0.00", CultureInfo.InvariantCulture);

        reserve.Status = ReserveStatus.Rejected;
        reserve.RejectedByUserId = request.ActorUserId;
        reserve.RejectedAtUtc = rejectedAtUtc;
        reserve.RejectionReason = reason;

        dbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ClaimId = request.ClaimId,
            Action = "ReserveRejected",
            ActorUserId = request.ActorUserId,
            CreatedAtUtc = rejectedAtUtc,
            Details = $"Reserve {reserve.Id} rejected for {formattedAmount} {reserve.Currency} by actor {request.ActorUserId}. Reason: {reason}"
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<RejectReserveResponse>.Success(new RejectReserveResponse(
            reserve.Id,
            reserve.ClaimId,
            reserve.Amount,
            reserve.Currency,
            reserve.Status,
            reserve.RejectedByUserId,
            reserve.RejectedAtUtc,
            reserve.RejectionReason));
    }
}
