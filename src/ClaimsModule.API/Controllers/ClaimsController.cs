using ClaimsModule.Application.Claims.CreateClaim;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims")]
public class ClaimsController(ISender sender) : ControllerBase
{
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
