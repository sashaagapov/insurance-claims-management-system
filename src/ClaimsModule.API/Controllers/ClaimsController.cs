using ClaimsModule.Application.Claims.CreateClaim;
using ClaimsModule.Application.Claims.GetClaimById;
using ClaimsModule.Application.Claims.GetClaims;
using ClaimsModule.Application.Claims.UpdateClaimStatus;
using ClaimsModule.Application.Reserves.ApproveReserve;
using ClaimsModule.Application.Reserves.CreateReserve;
using ClaimsModule.Application.Reserves.RejectReserve;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims")]
public class ClaimsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClaimListItemDto>>> GetClaims(
        CancellationToken cancellationToken)
    {
        var claims = await sender.Send(new GetClaimsQuery(), cancellationToken);
        return Ok(claims);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimDetailDto>> GetClaimById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var claim = await sender.Send(new GetClaimByIdQuery(id), cancellationToken);

        if (claim is null)
        {
            return NotFound(new { error = "Claim was not found." });
        }

        return Ok(claim);
    }

    [HttpPost]
    public async Task<ActionResult<CreateClaimResponse>> CreateClaim(
        CreateClaimCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Created($"/api/claims/{result.Value!.ClaimId}", result.Value);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<UpdateClaimStatusResponse>> UpdateClaimStatus(
        Guid id,
        UpdateClaimStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateClaimStatusCommand(id, request.NewStatus, request.ActorUserId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error?.StartsWith("NOT_FOUND:", StringComparison.Ordinal) == true)
        {
            return NotFound(new { error = result.Error["NOT_FOUND:".Length..].Trim() });
        }

        if (result.Error?.StartsWith("UNPROCESSABLE:", StringComparison.Ordinal) == true)
        {
            return UnprocessableEntity(new { error = result.Error["UNPROCESSABLE:".Length..].Trim() });
        }

        return BadRequest(new { error = result.Error?.Replace("BAD_REQUEST:", string.Empty).Trim() });
    }

    [HttpPost("{claimId:guid}/reserves")]
    public async Task<ActionResult<CreateReserveResponse>> CreateReserve(
        Guid claimId,
        CreateReserveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReserveCommand(
            claimId,
            request.Amount,
            request.Currency,
            request.CreatedByUserId);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Created($"/api/claims/{claimId}", result.Value);
        }

        if (result.Error?.StartsWith("NOT_FOUND:", StringComparison.Ordinal) == true)
        {
            return NotFound(new { error = result.Error["NOT_FOUND:".Length..].Trim() });
        }

        return BadRequest(new { error = result.Error?.Replace("BAD_REQUEST:", string.Empty).Trim() });
    }

    [HttpPatch("{claimId:guid}/reserves/{reserveId:guid}/approve")]
    public async Task<ActionResult<ApproveReserveResponse>> ApproveReserve(
        Guid claimId,
        Guid reserveId,
        ApproveReserveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ApproveReserveCommand(claimId, reserveId, request.ActorUserId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error?.StartsWith("NOT_FOUND:", StringComparison.Ordinal) == true)
        {
            return NotFound(new { error = result.Error["NOT_FOUND:".Length..].Trim() });
        }

        if (result.Error?.StartsWith("FORBIDDEN:", StringComparison.Ordinal) == true)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error["FORBIDDEN:".Length..].Trim() });
        }

        if (result.Error?.StartsWith("UNPROCESSABLE:", StringComparison.Ordinal) == true)
        {
            return UnprocessableEntity(new { error = result.Error["UNPROCESSABLE:".Length..].Trim() });
        }

        return BadRequest(new { error = result.Error?.Replace("BAD_REQUEST:", string.Empty).Trim() });
    }

    [HttpPatch("{claimId:guid}/reserves/{reserveId:guid}/reject")]
    public async Task<ActionResult<RejectReserveResponse>> RejectReserve(
        Guid claimId,
        Guid reserveId,
        RejectReserveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RejectReserveCommand(claimId, reserveId, request.ActorUserId, request.Reason);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error?.StartsWith("NOT_FOUND:", StringComparison.Ordinal) == true)
        {
            return NotFound(new { error = result.Error["NOT_FOUND:".Length..].Trim() });
        }

        if (result.Error?.StartsWith("FORBIDDEN:", StringComparison.Ordinal) == true)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error["FORBIDDEN:".Length..].Trim() });
        }

        if (result.Error?.StartsWith("UNPROCESSABLE:", StringComparison.Ordinal) == true)
        {
            return UnprocessableEntity(new { error = result.Error["UNPROCESSABLE:".Length..].Trim() });
        }

        return BadRequest(new { error = result.Error?.Replace("BAD_REQUEST:", string.Empty).Trim() });
    }
}

public record UpdateClaimStatusRequest(
    ClaimStatus NewStatus,
    Guid ActorUserId);

public record CreateReserveRequest(
    decimal Amount,
    string Currency,
    Guid CreatedByUserId);

public record ApproveReserveRequest(Guid ActorUserId);

public record RejectReserveRequest(
    Guid ActorUserId,
    string Reason);
