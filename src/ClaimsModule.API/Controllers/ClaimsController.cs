using ClaimsModule.Application.Claims.CreateClaim;
using ClaimsModule.Application.Claims.GetClaimById;
using ClaimsModule.Application.Claims.GetClaims;
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
}
