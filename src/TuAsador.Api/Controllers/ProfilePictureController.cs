using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Features.ProfilePicture.Commands.Delete;
using TuAsador.Application.Features.ProfilePicture.Commands.Upload;
using TuAsador.Application.Features.ProfilePicture.Queries.Get;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/profile-picture")]
public class ProfilePictureController : ControllerBase
{
    private readonly IMediator _mediator;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public ProfilePictureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return NotFound();
        }

        var result = await _mediator.Send(new GetProfilePictureQuery(userId));
        if (result == null)
            return NotFound();

        return File(result.Data, result.ContentType);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Debe seleccionar un archivo" });

        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "La imagen no puede superar los 5 MB" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { message = "Solo se permiten imágenes JPG, PNG y WebP" });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        try
        {
            await _mediator.Send(new UploadProfilePictureCommand(
                userId, ms.ToArray(), file.ContentType, file.FileName));
            var url = $"/api/profile-picture?userId={userId}";
            return Ok(new { profilePictureUrl = url });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _mediator.Send(new DeleteProfilePictureCommand(userId));
            return Ok(new { message = "Foto de perfil eliminada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
