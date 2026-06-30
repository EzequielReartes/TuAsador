using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.Ratings;
using TuAsador.Application.Features.Ratings.Commands.CreateRating;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/ratings")]
[Authorize(Roles = "Cliente")]
public class RatingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRatingRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var result = await _mediator.Send(new CreateRatingCommand(
                request.ContractId, userId,
                request.PunctualityScore, request.PresenceScore,
                request.PerformanceScore, request.Comment));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
