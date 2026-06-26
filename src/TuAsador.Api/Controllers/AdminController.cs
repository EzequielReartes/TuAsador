using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.Admin.Commands.ApprovePortfolioImage;
using TuAsador.Application.Features.Admin.Commands.RejectPortfolioImage;
using TuAsador.Application.Features.Admin.Commands.ToggleUserActive;
using TuAsador.Application.Features.Admin.Queries.GetPortfolioImages;
using TuAsador.Application.Features.Admin.Queries.GetUsers;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpPut("users/{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(string id)
    {
        try
        {
            var result = await _mediator.Send(new ToggleUserActiveCommand(id));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("portfolio-images")]
    public async Task<IActionResult> GetPortfolioImages([FromQuery] bool pendingOnly = true)
    {
        var result = await _mediator.Send(new GetPortfolioImagesQuery(pendingOnly));
        return Ok(result);
    }

    [HttpPut("portfolio-images/{id:guid}/approve")]
    public async Task<IActionResult> ApproveImage(Guid id)
    {
        try
        {
            await _mediator.Send(new ApprovePortfolioImageCommand(id));
            return Ok(new { message = "Imagen aprobada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("portfolio-images/{id:guid}/reject")]
    public async Task<IActionResult> RejectImage(Guid id)
    {
        try
        {
            await _mediator.Send(new RejectPortfolioImageCommand(id));
            return Ok(new { message = "Imagen rechazada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
