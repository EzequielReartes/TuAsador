using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.DTOs;
using TuAsador.Domain.Entities;
using TuAsador.Infrastructure.Data;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly TuAsadorDbContext _db;

    public AdminController(UserManager<User> userManager, TuAsadorDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Name)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email!,
                UserName = u.UserName!,
                Role = u.Role,
                PhoneNumber = u.PhoneNumber,
                WhatsApp = u.WhatsApp,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("users/{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = user.IsActive ? "Usuario habilitado" : "Usuario deshabilitado" });
    }

    [HttpGet("portfolio-images")]
    public async Task<IActionResult> GetPortfolioImages([FromQuery] bool pendingOnly = true)
    {
        var query = _db.PortfolioImages
            .Include(p => p.AsadorProfile)
            .ThenInclude(a => a.User)
            .AsQueryable();

        if (pendingOnly)
            query = query.Where(p => p.IsApproved == null);

        var images = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PortfolioImageDto
            {
                Id = p.Id,
                AsadorProfileId = p.AsadorProfileId,
                AsadorName = p.AsadorProfile.User.Name,
                ImageUrl = p.ImageUrl,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(images);
    }

    [HttpPut("portfolio-images/{id:guid}/approve")]
    public async Task<IActionResult> ApproveImage(Guid id)
    {
        var image = await _db.PortfolioImages.FindAsync(id);
        if (image == null)
            return NotFound(new { message = "Imagen no encontrada" });

        image.IsApproved = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Imagen aprobada" });
    }

    [HttpPut("portfolio-images/{id:guid}/reject")]
    public async Task<IActionResult> RejectImage(Guid id)
    {
        var image = await _db.PortfolioImages.FindAsync(id);
        if (image == null)
            return NotFound(new { message = "Imagen no encontrada" });

        image.IsApproved = false;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Imagen rechazada" });
    }
}
