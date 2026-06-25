using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.DTOs;
using TuAsador.Domain.Entities;
using TuAsador.Infrastructure.Data;

namespace TuAsador.Api.Controllers;

[ApiController]
[Route("api/asador/profile")]
public class AsadorProfileController : ControllerBase
{
    private readonly TuAsadorDbContext _db;
    private readonly UserManager<User> _userManager;

    public AsadorProfileController(TuAsadorDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles
            .Include(p => p.User)
            .Include(p => p.Specialties)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        return Ok(new AsadorProfileResponse
        {
            Id = profile.Id,
            Name = profile.User.Name,
            Email = profile.User.Email!,
            WhatsApp = profile.User.WhatsApp,
            Description = profile.Description,
            Instagram = profile.Instagram,
            PhotoUrl = profile.PhotoUrl,
            MainCity = profile.MainCity,
            Status = profile.Status,
            SpecialtyIds = profile.Specialties.Select(s => s.Id).ToList(),
            SpecialtyNames = profile.Specialties.Select(s => s.Name).ToList()
        });
    }

    [HttpPut]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Update([FromBody] UpdateAsadorProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles
            .Include(p => p.User)
            .Include(p => p.Specialties)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return NotFound(new { message = "Perfil de asador no encontrado" });

        profile.Description = request.Description;
        profile.Instagram = request.Instagram;
        profile.PhotoUrl = request.PhotoUrl;
        profile.MainCity = request.MainCity;

        if (request.WhatsApp != null)
            profile.User.WhatsApp = request.WhatsApp;

        if (request.SpecialtyIds.Count != 0)
        {
            var specialties = await _db.Specialties
                .Where(s => request.SpecialtyIds.Contains(s.Id))
                .ToListAsync();
            profile.Specialties.Clear();
            foreach (var s in specialties)
                profile.Specialties.Add(s);
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Perfil actualizado correctamente" });
    }
}
