namespace ClaimsModule.Application.CauseOfLossCodes.GetCauseOfLossCodes;

public record CauseOfLossCodeDto(
    Guid Id,
    string Code,
    string Description);
