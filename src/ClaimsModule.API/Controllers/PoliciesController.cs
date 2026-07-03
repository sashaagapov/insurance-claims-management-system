using ClaimsModule.Application.Policies.GetPolicies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/policies")]
public class PoliciesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PolicyListItemDto>>> GetPolicies(
        CancellationToken cancellationToken)
    {
        var policies = await sender.Send(new GetPoliciesQuery(), cancellationToken);
        return Ok(policies);
    }
}
