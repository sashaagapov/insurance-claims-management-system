namespace ClaimsModule.Application.Claims.GetClaimById;

public record AuditLogEntryDto(
    Guid Id,
    string Action,
    Guid ActorUserId,
    DateTime CreatedAtUtc,
    string? Details);
