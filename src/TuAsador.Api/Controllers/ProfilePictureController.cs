using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Domain.Entities;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/profile-picture")]
[Authorize]
public class ProfilePictureController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public ProfilePictureController(UserManager<User> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }

    [HttpPost]
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "profiles");
        Directory.CreateDirectory(uploadsDir);

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            var oldFile = Path.Combine(
                _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
                user.ProfilePictureUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );
            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);
        }

        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfilePictureUrl = $"/uploads/profiles/{uniqueName}";
        await _userManager.UpdateAsync(user);

        return Ok(new { profilePictureUrl = user.ProfilePictureUrl });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            var filePath = Path.Combine(
                _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
                user.ProfilePictureUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            user.ProfilePictureUrl = null;
            await _userManager.UpdateAsync(user);
        }

        return Ok(new { message = "Foto de perfil eliminada" });
    }
}
