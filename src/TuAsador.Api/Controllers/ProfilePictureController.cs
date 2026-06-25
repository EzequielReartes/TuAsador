using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TuAsador.Domain.Entities;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/profile-picture")]
public class ProfilePictureController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public ProfilePictureController(UserManager<User> userManager)
    {
        _userManager = userManager;
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

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ProfilePictureData == null || user.ProfilePictureContentType == null)
            return NotFound();

        return File(user.ProfilePictureData, user.ProfilePictureContentType);
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        user.ProfilePictureData = ms.ToArray();
        user.ProfilePictureContentType = file.ContentType;
        user.ProfilePictureUrl = $"/api/profile-picture?userId={userId}";

        await _userManager.UpdateAsync(user);

        return Ok(new { profilePictureUrl = user.ProfilePictureUrl });
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        user.ProfilePictureData = null;
        user.ProfilePictureContentType = null;
        user.ProfilePictureUrl = null;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Foto de perfil eliminada" });
    }
}
