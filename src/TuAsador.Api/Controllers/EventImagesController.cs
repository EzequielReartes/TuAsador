using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuAsador.Domain.Entities;
using TuAsador.Infrastructure.Data;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/images")]
public class EventImagesController : ControllerBase
{
    private readonly TuAsadorDbContext _db;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public EventImagesController(TuAsadorDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Upload(Guid eventId, List<IFormFile> files)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var domainEvent = await _db.Events
            .Include(e => e.Images)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        if (domainEvent.ClientId != userId)
            return Forbid();

        if (files == null || files.Count == 0)
            return BadRequest(new { message = "Debe seleccionar al menos un archivo" });

        var existingCount = domainEvent.Images.Count;
        var maxNew = 3 - existingCount;

        if (maxNew <= 0)
            return BadRequest(new { message = "El evento ya tiene el máximo de 3 imágenes" });

        if (files.Count > maxNew)
            return BadRequest(new { message = $"Solo puede agregar {maxNew} imagen(es) más. Máximo 3 por evento" });

        var uploadsDir = Path.Combine(
            _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            "uploads", "events"
        );
        Directory.CreateDirectory(uploadsDir);

        var images = new List<EventImage>();

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = $"La imagen '{file.FileName}' no puede superar los 5 MB" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Solo se permiten imágenes JPG, PNG y WebP. Archivo '{file.FileName}' no válido" });

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            images.Add(new EventImage
            {
                EventId = eventId,
                ImageUrl = $"/uploads/events/{uniqueName}"
            });
        }

        _db.EventImages.AddRange(images);
        await _db.SaveChangesAsync();

        var result = images.Select(i => new { i.Id, i.ImageUrl, i.CreatedAt }).ToList();

        return Ok(result);
    }

    [HttpDelete("{imageId:guid}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Delete(Guid eventId, Guid imageId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var domainEvent = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        if (domainEvent.ClientId != userId)
            return Forbid();

        var image = await _db.EventImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.EventId == eventId);

        if (image == null)
            return NotFound(new { message = "Imagen no encontrada" });

        var filePath = Path.Combine(
            _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            image.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        );

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _db.EventImages.Remove(image);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Imagen eliminada" });
    }
}
