using ClaimsModule.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.GetClaimById;

public class GetClaimByIdQueryHandler(IClaimsModuleDbContext dbContext)
    : IRequestHandler<GetClaimByIdQuery, ClaimDetailDto?>
{
    public async Task<ClaimDetailDto?> Handle(
        GetClaimByIdQuery request,
        CancellationToken cancellationToken)
    {
        var claim = await dbContext.Claims
            .AsNoTracking()
            .Where(claim => claim.Id == request.ClaimId)
            .Select(claim => new
            {
                claim.Id,
                claim.ClaimNumber,
                claim.PolicyId,
                PolicyNumber = claim.Policy!.PolicyNumber,
                PolicyholderName = claim.Policy.PolicyholderName,
                ProductType = claim.Policy.ProductType,
                claim.CauseOfLossCodeId,
                CauseOfLossCode = claim.CauseOfLossCode!.Code,
                CauseOfLossDescription = claim.CauseOfLossCode.Description,
                claim.LossDate,
                claim.ReportedAtUtc,
                claim.Description,
                claim.Status,
                claim.CreatedByUserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (claim is null)
        {
            return null;
        }

        var parties = await dbContext.ClaimParties
            .AsNoTracking()
            .Where(party => party.ClaimId == request.ClaimId)
            .OrderBy(party => party.FullName)
            .Select(party => new ClaimPartyDetailDto(
                party.Id,
                party.PartyType,
                party.FullName,
                party.Email,
                party.Phone,
                party.Notes))
            .ToListAsync(cancellationToken);

        var riskObjects = await dbContext.RiskObjects
            .AsNoTracking()
            .Where(riskObject => riskObject.ClaimId == request.ClaimId)
            .OrderBy(riskObject => riskObject.Description)
            .Select(riskObject => new RiskObjectDetailDto(
                riskObject.Id,
                riskObject.ObjectType,
                riskObject.ExternalReference,
                riskObject.Description))
            .ToListAsync(cancellationToken);

        var reserves = await dbContext.Reserves
            .AsNoTracking()
            .Where(reserve => reserve.ClaimId == request.ClaimId)
            .OrderByDescending(reserve => reserve.CreatedAtUtc)
            .Select(reserve => new ReserveSummaryDto(
                reserve.Id,
                reserve.Amount,
                reserve.Currency,
                reserve.Status,
                reserve.CreatedAtUtc,
                reserve.ApprovedByUserId,
                reserve.ApprovedAtUtc,
                reserve.RejectedByUserId,
                reserve.RejectedAtUtc))
            .ToListAsync(cancellationToken);

        var auditLogEntries = await dbContext.AuditLogEntries
            .AsNoTracking()
            .Where(entry => entry.ClaimId == request.ClaimId)
            .OrderByDescending(entry => entry.CreatedAtUtc)
            .Select(entry => new AuditLogEntryDto(
                entry.Id,
                entry.Action,
                entry.ActorUserId,
                entry.CreatedAtUtc,
                entry.Details))
            .ToListAsync(cancellationToken);

        return new ClaimDetailDto(
            claim.Id,
            claim.ClaimNumber,
            claim.PolicyId,
            claim.PolicyNumber,
            claim.PolicyholderName,
            claim.ProductType,
            claim.CauseOfLossCodeId,
            claim.CauseOfLossCode,
            claim.CauseOfLossDescription,
            claim.LossDate,
            claim.ReportedAtUtc,
            claim.Description,
            claim.Status,
            claim.CreatedByUserId,
            parties,
            riskObjects,
            reserves,
            auditLogEntries);
    }
}
