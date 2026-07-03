using System.Globalization;
using ClaimsModule.Application.Common;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.ApproveReserve;

public class ApproveReserveCommandHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<ApproveReserveCommand, Result<ApproveReserveResponse>>
{
    public async Task<Result<ApproveReserveResponse>> Handle(
        ApproveReserveCommand request,
        CancellationToken cancellationToken)
    {
        var claimExists = await dbContext.Claims
            .AsNoTracking()
            .AnyAsync(claim => claim.Id == request.ClaimId, cancellationToken);

        if (!claimExists)
        {
            return Result<ApproveReserveResponse>.Failure("NOT_FOUND: Claim was not found.");
        }

        var reserve = await dbContext.Reserves
            .FirstOrDefaultAsync(
                item => item.Id == request.ReserveId && item.ClaimId == request.ClaimId,
                cancellationToken);

        if (reserve is null)
        {
            return Result<ApproveReserveResponse>.Failure("NOT_FOUND: Reserve was not found for this claim.");
        }

        var actor = await dbContext.MockUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Id == request.ActorUserId && user.IsActive,
                cancellationToken);

        if (actor is null)
        {
            return Result<ApproveReserveResponse>.Failure("BAD_REQUEST: Actor user was not found or is not active.");
        }

        if (reserve.CreatedByUserId == request.ActorUserId)
        {
            return Result<ApproveReserveResponse>.Failure("UNPROCESSABLE: Users cannot approve their own reserve.");
        }

        if (actor.Role is not UserRole.Supervisor and not UserRole.Manager)
        {
            return Result<ApproveReserveResponse>.Failure("FORBIDDEN: Only a supervisor or manager can approve reserves.");
        }

        if (reserve.Status != ReserveStatus.PendingApproval)
        {
            return Result<ApproveReserveResponse>.Failure(
                $"UNPROCESSABLE: Only PendingApproval reserves can be approved. Current status is {reserve.Status}.");
        }

        var approvedAtUtc = DateTime.UtcNow;
        var formattedAmount = reserve.Amount.ToString("0.00", CultureInfo.InvariantCulture);

        reserve.Status = ReserveStatus.Approved;
        reserve.ApprovedByUserId = request.ActorUserId;
        reserve.ApprovedAtUtc = approvedAtUtc;

        dbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ClaimId = request.ClaimId,
            Action = "ReserveApproved",
            ActorUserId = request.ActorUserId,
            CreatedAtUtc = approvedAtUtc,
            Details = $"Reserve {reserve.Id} approved for {formattedAmount} {reserve.Currency} by actor {request.ActorUserId}."
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ApproveReserveResponse>.Success(new ApproveReserveResponse(
            reserve.Id,
            reserve.ClaimId,
            reserve.Amount,
            reserve.Currency,
            reserve.Status,
            reserve.ApprovedByUserId,
            reserve.ApprovedAtUtc));
    }
}
