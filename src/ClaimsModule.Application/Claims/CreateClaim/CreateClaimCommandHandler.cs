using ClaimsModule.Application.Common;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.CreateClaim;

public class CreateClaimCommandHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<CreateClaimCommand, Result<CreateClaimResponse>>
{
    public async Task<Result<CreateClaimResponse>> Handle(
        CreateClaimCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await dbContext.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.PolicyId, cancellationToken);

        if (policy is null)
        {
            return Result<CreateClaimResponse>.Failure("Policy was not found.");
        }

        if (policy.Status != PolicyStatus.Active)
        {
            return Result<CreateClaimResponse>.Failure("Policy is not active.");
        }

        var causeOfLossCode = await dbContext.CauseOfLossCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CauseOfLossCodeId, cancellationToken);

        if (causeOfLossCode is null)
        {
            return Result<CreateClaimResponse>.Failure("Cause of loss code was not found.");
        }

        if (!causeOfLossCode.IsActive)
        {
            return Result<CreateClaimResponse>.Failure("Cause of loss code is not active.");
        }

        var createdByUserExists = await dbContext.MockUsers
            .AsNoTracking()
            .AnyAsync(user => user.Id == request.CreatedByUserId && user.IsActive, cancellationToken);

        if (!createdByUserExists)
        {
            return Result<CreateClaimResponse>.Failure("Created by user was not found or is not active.");
        }

        var nowUtc = DateTime.UtcNow;
        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            ClaimNumber = GenerateClaimNumber(nowUtc),
            PolicyId = request.PolicyId,
            CauseOfLossCodeId = request.CauseOfLossCodeId,
            LossDate = request.LossDate,
            ReportedAtUtc = nowUtc,
            Description = request.Description.Trim(),
            Status = ClaimStatus.Open,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAtUtc = nowUtc
        };

        foreach (var party in request.Parties)
        {
            claim.Parties.Add(new ClaimParty
            {
                Id = Guid.NewGuid(),
                PartyType = party.PartyType,
                FullName = party.FullName.Trim(),
                Email = party.Email?.Trim(),
                Phone = party.Phone?.Trim(),
                Notes = party.Notes?.Trim()
            });
        }

        foreach (var riskObject in request.RiskObjects)
        {
            claim.RiskObjects.Add(new RiskObject
            {
                Id = Guid.NewGuid(),
                ObjectType = riskObject.ObjectType,
                ExternalReference = riskObject.ExternalReference?.Trim(),
                Description = riskObject.Description.Trim()
            });
        }

        claim.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            Action = "ClaimCreated",
            ActorUserId = request.CreatedByUserId,
            CreatedAtUtc = nowUtc,
            Details = $"Claim {claim.ClaimNumber} was created through FNOL."
        });

        dbContext.Claims.Add(claim);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<CreateClaimResponse>.Success(new CreateClaimResponse(
            claim.Id,
            claim.ClaimNumber,
            claim.Status,
            claim.ReportedAtUtc));
    }

    private static string GenerateClaimNumber(DateTime nowUtc)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"CLM-{nowUtc:yyyyMMdd}-{suffix}";
    }
}
