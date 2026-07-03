using ClaimsModule.Application.Common;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.UpdateClaimStatus;

public class UpdateClaimStatusCommandHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<UpdateClaimStatusCommand, Result<UpdateClaimStatusResponse>>
{
    private static readonly IReadOnlySet<(ClaimStatus From, ClaimStatus To)> AllowedTransitions =
        new HashSet<(ClaimStatus From, ClaimStatus To)>
        {
            (ClaimStatus.Draft, ClaimStatus.Open),
            (ClaimStatus.Open, ClaimStatus.UnderInvestigation),
            (ClaimStatus.UnderInvestigation, ClaimStatus.PendingPayment),
            (ClaimStatus.PendingPayment, ClaimStatus.Closed),
            (ClaimStatus.Closed, ClaimStatus.Reopened),
            (ClaimStatus.Reopened, ClaimStatus.UnderInvestigation),
            (ClaimStatus.Open, ClaimStatus.Withdrawn),
            (ClaimStatus.UnderInvestigation, ClaimStatus.Withdrawn)
        };

    public async Task<Result<UpdateClaimStatusResponse>> Handle(
        UpdateClaimStatusCommand request,
        CancellationToken cancellationToken)
    {
        var claim = await dbContext.Claims
            .FirstOrDefaultAsync(item => item.Id == request.ClaimId, cancellationToken);

        if (claim is null)
        {
            return Result<UpdateClaimStatusResponse>.Failure("NOT_FOUND: Claim was not found.");
        }

        var actorExists = await dbContext.MockUsers
            .AsNoTracking()
            .AnyAsync(user => user.Id == request.ActorUserId && user.IsActive, cancellationToken);

        if (!actorExists)
        {
            return Result<UpdateClaimStatusResponse>.Failure("BAD_REQUEST: Actor user was not found or is not active.");
        }

        if (claim.Status == request.NewStatus)
        {
            return Result<UpdateClaimStatusResponse>.Failure("UNPROCESSABLE: New status must be different from the current status.");
        }

        if (!IsTransitionAllowed(claim.Status, request.NewStatus))
        {
            return Result<UpdateClaimStatusResponse>.Failure(
                $"UNPROCESSABLE: Transition from {claim.Status} to {request.NewStatus} is not allowed.");
        }

        var oldStatus = claim.Status;
        var changedAtUtc = DateTime.UtcNow;

        claim.Status = request.NewStatus;
        dbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ClaimId = claim.Id,
            Action = "ClaimStatusChanged",
            ActorUserId = request.ActorUserId,
            CreatedAtUtc = changedAtUtc,
            Details = $"Claim status changed from {oldStatus} to {request.NewStatus}."
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UpdateClaimStatusResponse>.Success(new UpdateClaimStatusResponse(
            claim.Id,
            claim.ClaimNumber,
            oldStatus,
            claim.Status,
            changedAtUtc));
    }

    private static bool IsTransitionAllowed(ClaimStatus currentStatus, ClaimStatus newStatus) =>
        AllowedTransitions.Contains((currentStatus, newStatus));
}
