using ClaimsModule.Application.Interfaces;
using Hangfire;

namespace ClaimsModule.Infrastructure.BackgroundJobs;

public class ReserveGlPostingJobQueue(IBackgroundJobClient backgroundJobClient)
    : IReserveGlPostingJobQueue
{
    public void EnqueueReservePosting(Guid reserveId)
    {
        backgroundJobClient.Enqueue<ReserveGlPostingJob>(
            job => job.PostReserveAsync(reserveId));
    }
}
