using MediatR;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.Health.Queries.GetHealth;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetHealthQuery());
        return Ok(result);
    }
}
