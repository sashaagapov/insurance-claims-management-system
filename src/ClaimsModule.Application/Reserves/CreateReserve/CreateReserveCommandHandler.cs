using System.Globalization;
using ClaimsModule.Application.Common;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.CreateReserve;

public class CreateReserveCommandHandler(
    IClaimsModuleDbContext dbContext,
    IReserveGlPostingJobQueue reserveGlPostingJobQueue)
    : IRequestHandler<CreateReserveCommand, Result<CreateReserveResponse>>
{
    private const decimal AutoApprovalThreshold = 10000m;

    public async Task<Result<CreateReserveResponse>> Handle(
        CreateReserveCommand request,
        CancellationToken cancellationToken)
    {
        var claimExists = await dbContext.Claims
            .AsNoTracking()
            .AnyAsync(claim => claim.Id == request.ClaimId, cancellationToken);

        if (!claimExists)
        {
            return Result<CreateReserveResponse>.Failure("NOT_FOUND: Claim was not found.");
        }

        var creatorExists = await dbContext.MockUsers
            .AsNoTracking()
            .AnyAsync(user => user.Id == request.CreatedByUserId && user.IsActive, cancellationToken);

        if (!creatorExists)
        {
            return Result<CreateReserveResponse>.Failure("BAD_REQUEST: Created by user was not found or is not active.");
        }

        var createdAtUtc = DateTime.UtcNow;
        var currency = request.Currency.Trim();
        var formattedAmount = request.Amount.ToString("0.00", CultureInfo.InvariantCulture);
        var status = request.Amount <= AutoApprovalThreshold
            ? ReserveStatus.Approved
            : ReserveStatus.PendingApproval;

        var reserve = new Reserve
        {
            Id = Guid.NewGuid(),
            ClaimId = request.ClaimId,
            Amount = request.Amount,
            Currency = currency,
            Status = status,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAtUtc = createdAtUtc
        };

        dbContext.Reserves.Add(reserve);
        dbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ClaimId = request.ClaimId,
            Action = "ReserveCreated",
            ActorUserId = request.CreatedByUserId,
            CreatedAtUtc = createdAtUtc,
            Details = $"Reserve created for {formattedAmount} {currency} with status {status}."
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        if (reserve.Status == ReserveStatus.Approved)
        {
            reserveGlPostingJobQueue.EnqueueReservePosting(reserve.Id);
        }

        return Result<CreateReserveResponse>.Success(new CreateReserveResponse(
            reserve.Id,
            reserve.ClaimId,
            reserve.Amount,
            reserve.Currency,
            reserve.Status,
            reserve.CreatedAtUtc));
    }
}
