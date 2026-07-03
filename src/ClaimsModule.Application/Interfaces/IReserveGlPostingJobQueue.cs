namespace ClaimsModule.Application.Interfaces;

public interface IReserveGlPostingJobQueue
{
    void EnqueueReservePosting(Guid reserveId);
}
