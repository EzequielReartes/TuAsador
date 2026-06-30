using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.AsadorProfile.Commands.UpdateProfile;
using TuAsador.Application.Features.AsadorProfile.Queries.GetMyProfile;
using TuAsador.Application.Features.AsadorProfile.Queries.GetPublicProfile;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/asador/profile")]
public class AsadorProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public AsadorProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetMyProfileQuery(userId));

        if (result == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        return Ok(result);
    }

    [HttpGet("{profileId:guid}/public")]
    [Authorize]
    public async Task<IActionResult> GetPublic(Guid profileId)
    {
        var result = await _mediator.Send(new GetPublicProfileQuery(profileId));

        if (result == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Update([FromBody] UpdateAsadorProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _mediator.Send(new UpdateProfileCommand(
                userId, request.Description, request.Instagram, request.PhotoUrl,
                request.MainCity, request.WhatsApp, request.SpecialtyIds));
            return Ok(new { message = "Perfil actualizado correctamente" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
