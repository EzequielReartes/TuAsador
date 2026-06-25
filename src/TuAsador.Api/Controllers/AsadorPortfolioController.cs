using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuAsador.Domain.Entities;
using TuAsador.Infrastructure.Data;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/asador/portfolio")]
[Authorize(Roles = "Asador")]
public class AsadorPortfolioController : ControllerBase
{
    private readonly TuAsadorDbContext _db;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public AsadorPortfolioController(TuAsadorDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyImages()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        var images = await _db.PortfolioImages
            .Where(p => p.AsadorProfileId == profile.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.ImageUrl,
                p.IsApproved,
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(images);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { message = "Debe seleccionar al menos un archivo" });

        if (files.Count > 5)
            return BadRequest(new { message = "Máximo 5 imágenes por subida" });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "portfolio");
        Directory.CreateDirectory(uploadsDir);

        var images = new List<PortfolioImage>();

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

            images.Add(new PortfolioImage
            {
                AsadorProfileId = profile.Id,
                ImageUrl = $"/uploads/portfolio/{uniqueName}",
                IsApproved = null
            });
        }

        _db.PortfolioImages.AddRange(images);
        await _db.SaveChangesAsync();

        return Ok(images.Select(i => new
        {
            i.Id,
            i.ImageUrl,
            i.IsApproved,
            i.CreatedAt
        }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        var image = await _db.PortfolioImages
            .FirstOrDefaultAsync(p => p.Id == id && p.AsadorProfileId == profile.Id);

        if (image == null)
            return NotFound(new { message = "Imagen no encontrada" });

        var filePath = Path.Combine(
            _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            image.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        );

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _db.PortfolioImages.Remove(image);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Imagen eliminada" });
    }
}
