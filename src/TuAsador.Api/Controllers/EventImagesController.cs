using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Common.Models;
using TuAsador.Application.Features.Events.Commands.DeleteImage;
using TuAsador.Application.Features.Events.Commands.UploadImages;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/images")]
public class EventImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventImagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Upload(Guid eventId, List<IFormFile> files)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var fileData = new List<FileData>();
        if (files != null)
        {
            foreach (var f in files)
            {
                using var ms = new MemoryStream();
                await f.CopyToAsync(ms);
                fileData.Add(new FileData(ms.ToArray(), f.FileName, f.Length));
            }
        }

        try
        {
            var result = await _mediator.Send(new UploadEventImagesCommand(eventId, userId, fileData));
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{imageId:guid}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Delete(Guid eventId, Guid imageId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _mediator.Send(new DeleteEventImageCommand(eventId, imageId, userId));
            return Ok(new { message = "Imagen eliminada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
