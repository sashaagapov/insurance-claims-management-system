using ClaimsModule.Application.CauseOfLossCodes.GetCauseOfLossCodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/cause-of-loss-codes")]
public class CauseOfLossCodesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CauseOfLossCodeDto>>> GetCauseOfLossCodes(
        CancellationToken cancellationToken)
    {
        var codes = await sender.Send(new GetCauseOfLossCodesQuery(), cancellationToken);
        return Ok(codes);
    }
}
