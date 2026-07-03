using System.Globalization;
using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Infrastructure.BackgroundJobs;

public class ReserveGlPostingJob(IClaimsModuleDbContext dbContext)
{
    public async Task PostReserveAsync(Guid reserveId)
    {
        var reserve = await dbContext.Reserves
            .FirstOrDefaultAsync(item => item.Id == reserveId);

        if (reserve is null)
        {
            return;
        }

        if (reserve.Status != ReserveStatus.Approved || reserve.GlPostedAtUtc is not null)
        {
            return;
        }

        var postedAtUtc = DateTime.UtcNow;
        var glReference = $"GL-{postedAtUtc:yyyyMMdd}-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
        var formattedAmount = reserve.Amount.ToString("0.00", CultureInfo.InvariantCulture);

        reserve.GlPostedAtUtc = postedAtUtc;
        reserve.GlPostingReference = glReference;

        dbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ClaimId = reserve.ClaimId,
            Action = "ReserveGlPosted",
            ActorUserId = reserve.ApprovedByUserId ?? reserve.CreatedByUserId,
            CreatedAtUtc = postedAtUtc,
            Details = $"Reserve {reserve.Id} posted to simulated GL for {formattedAmount} {reserve.Currency}. Reference: {glReference}."
        });

        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
