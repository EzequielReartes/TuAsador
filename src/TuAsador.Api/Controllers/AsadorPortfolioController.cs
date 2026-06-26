using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Application.Common.Models;
using TuAsador.Application.Features.Portfolio.Commands.DeleteImage;
using TuAsador.Application.Features.Portfolio.Commands.UploadImages;
using TuAsador.Application.Features.Portfolio.Queries.GetMyImages;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/asador/portfolio")]
[Authorize(Roles = "Asador")]
public class AsadorPortfolioController : ControllerBase
{
    private readonly IMediator _mediator;

    public AsadorPortfolioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyImages()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetMyPortfolioImagesQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
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
            var result = await _mediator.Send(new UploadPortfolioImagesCommand(userId, fileData));
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _mediator.Send(new DeletePortfolioImageCommand(id, userId));
            return Ok(new { message = "Imagen eliminada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
