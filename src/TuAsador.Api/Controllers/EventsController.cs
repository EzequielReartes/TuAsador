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
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly TuAsadorDbContext _db;
    private readonly UserManager<User> _userManager;

    public EventsController(TuAsadorDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        var query = _db.Events
            .Include(e => e.Client)
            .Include(e => e.Applications)
            .Include(e => e.Images)
            .Where(e => e.Status == "Disponible");

        if (profile != null)
            query = query.Where(e => !e.Applications.Any(a => a.AsadorProfileId == profile.Id));

        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        var result = events.Select(e => new EventDto
        {
            Id = e.Id,
            ClientId = e.ClientId,
            ClientName = e.Client.Name,
            Date = e.Date,
            Time = e.Time,
            City = e.City,
            Address = e.Address,
            PeopleCount = e.PeopleCount,
            EventType = e.EventType,
            ServiceDesired = e.ServiceDesired,
            Notes = e.Notes,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            ApplicationCount = e.Applications.Count,
            ImageUrls = e.Images.Select(i => i.ImageUrl).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var events = await _db.Events
            .Include(e => e.Client)
            .Include(e => e.Applications)
            .Include(e => e.Images)
            .Where(e => e.ClientId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        var result = events.Select(e => new EventDto
        {
            Id = e.Id,
            ClientId = e.ClientId,
            ClientName = e.Client.Name,
            Date = e.Date,
            Time = e.Time,
            City = e.City,
            Address = e.Address,
            PeopleCount = e.PeopleCount,
            EventType = e.EventType,
            ServiceDesired = e.ServiceDesired,
            Notes = e.Notes,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            ApplicationCount = e.Applications.Count,
            ImageUrls = e.Images.Select(i => i.ImageUrl).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("applied")]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> GetApplied()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return BadRequest(new { message = "Perfil de asador no encontrado" });

        var events = await _db.EventApplications
            .Include(a => a.Event).ThenInclude(e => e.Client)
            .Include(a => a.Event).ThenInclude(e => e.Applications)
            .Include(a => a.Event).ThenInclude(e => e.Images)
            .Where(a => a.AsadorProfileId == profile.Id)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new EventDto
            {
                Id = a.Event.Id,
                ClientId = a.Event.ClientId,
                ClientName = a.Event.Client.Name,
                Date = a.Event.Date,
                Time = a.Event.Time,
                City = a.Event.City,
                Address = a.Event.Address,
                PeopleCount = a.Event.PeopleCount,
                EventType = a.Event.EventType,
                ServiceDesired = a.Event.ServiceDesired,
                Notes = a.Event.Notes,
                Status = a.Event.Status,
                CreatedAt = a.Event.CreatedAt,
                ApplicationCount = a.Event.Applications.Count,
                ImageUrls = a.Event.Images.Select(i => i.ImageUrl).ToList(),
                ApplicationStatus = a.Status
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        var domainEvent = await _db.Events
            .Include(e => e.Client)
            .Include(e => e.Images)
            .Include(e => e.Applications)
                .ThenInclude(a => a.AsadorProfile)
                    .ThenInclude(ap => ap.User)
            .Include(e => e.Applications)
                .ThenInclude(a => a.AsadorProfile)
                    .ThenInclude(ap => ap.Specialties)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        var isOwner = userId == domainEvent.ClientId;

        var result = new EventDetailDto
        {
            Id = domainEvent.Id,
            ClientId = domainEvent.ClientId,
            ClientName = domainEvent.Client.Name,
            Date = domainEvent.Date,
            Time = domainEvent.Time,
            City = domainEvent.City,
            Address = domainEvent.Address,
            PeopleCount = domainEvent.PeopleCount,
            EventType = domainEvent.EventType,
            ServiceDesired = domainEvent.ServiceDesired,
            Notes = domainEvent.Notes,
            Status = domainEvent.Status,
            CreatedAt = domainEvent.CreatedAt,
            HasApplied = profile != null && domainEvent.Applications.Any(a => a.AsadorProfileId == profile.Id),
            ImageUrls = domainEvent.Images.Select(i => i.ImageUrl).ToList(),
            Applications = isOwner
                ? domainEvent.Applications.Select(a => new EventApplicationDto
                {
                    Id = a.Id,
                    AsadorProfileId = a.AsadorProfileId,
                    AsadorName = a.AsadorProfile.User.Name,
                    AsadorPhotoUrl = a.AsadorProfile.PhotoUrl,
                    WhatsApp = a.AsadorProfile.User.WhatsApp,
                    MainCity = a.AsadorProfile.MainCity,
                    AverageRating = a.AsadorProfile.AverageRating,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    Description = a.AsadorProfile.Description,
                    SpecialtyNames = a.AsadorProfile.Specialties.Select(s => s.Name).ToList()
                }).ToList()
                : new List<EventApplicationDto>()
        };

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var domainEvent = new TuAsador.Domain.Entities.Event
        {
            ClientId = userId,
            Date = request.Date,
            Time = request.Time,
            City = request.City,
            Address = request.Address,
            PeopleCount = request.PeopleCount,
            EventType = request.EventType,
            ServiceDesired = request.ServiceDesired,
            Notes = request.Notes,
            Status = "Disponible"
        };

        _db.Events.Add(domainEvent);
        await _db.SaveChangesAsync();

        return Ok(new { id = domainEvent.Id, message = "Evento creado correctamente" });
    }

    [HttpPost("{id:guid}/apply")]
    [Authorize(Roles = "Asador")]
    public async Task<IActionResult> Apply(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return BadRequest(new { message = "Perfil de asador no encontrado" });

        var domainEvent = await _db.Events
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        if (domainEvent.Status != "Disponible")
            return BadRequest(new { message = "El evento ya no está disponible" });

        if (domainEvent.Applications.Any(a => a.AsadorProfileId == profile.Id))
            return BadRequest(new { message = "Ya te has postulado a este evento" });

        var application = new EventApplication
        {
            EventId = id,
            AsadorProfileId = profile.Id
        };

        _db.EventApplications.Add(application);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Postulación enviada correctamente" });
    }

    [HttpGet("{id:guid}/applications")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetApplications(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var domainEvent = await _db.Events.FirstOrDefaultAsync(e => e.Id == id);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        if (domainEvent.ClientId != userId)
            return Forbid();

        var applications = await _db.EventApplications
            .Include(a => a.AsadorProfile)
                .ThenInclude(ap => ap.User)
            .Include(a => a.AsadorProfile)
                .ThenInclude(ap => ap.Specialties)
            .Where(a => a.EventId == id)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        var result = applications.Select(a => new EventApplicationDto
        {
            Id = a.Id,
            AsadorProfileId = a.AsadorProfileId,
            AsadorName = a.AsadorProfile.User.Name,
            AsadorPhotoUrl = a.AsadorProfile.PhotoUrl,
            WhatsApp = a.AsadorProfile.User.WhatsApp,
            MainCity = a.AsadorProfile.MainCity,
            AverageRating = a.AsadorProfile.AverageRating,
            Status = a.Status,
            CreatedAt = a.CreatedAt,
            Description = a.AsadorProfile.Description,
            SpecialtyNames = a.AsadorProfile.Specialties.Select(s => s.Name).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpPut("{id:guid}/applications/{applicationId:guid}/select")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> SelectApplication(Guid id, Guid applicationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var domainEvent = await _db.Events
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (domainEvent == null)
            return NotFound(new { message = "Evento no encontrado" });

        if (domainEvent.ClientId != userId)
            return Forbid();

        if (domainEvent.Status != "Disponible")
            return BadRequest(new { message = "El evento ya no está disponible" });

        var application = domainEvent.Applications
            .FirstOrDefault(a => a.Id == applicationId);

        if (application == null)
            return NotFound(new { message = "Postulación no encontrada" });

        if (application.Status != "Pendiente")
            return BadRequest(new { message = "Esta postulación ya fue procesada" });

        application.Status = "Aceptada";
        domainEvent.Status = "Asignado";

        foreach (var other in domainEvent.Applications.Where(a => a.Id != applicationId))
        {
            other.Status = "Rechazada";
        }

        var contract = new Contract
        {
            ClientId = userId,
            AsadorProfileId = application.AsadorProfileId,
            EventId = id,
            Type = "Evento",
            Date = domainEvent.Date,
            Time = domainEvent.Time,
            Address = domainEvent.Address,
            City = domainEvent.City,
            PeopleCount = domainEvent.PeopleCount,
            EventType = domainEvent.EventType,
            ServiceDesired = domainEvent.ServiceDesired,
            Notes = domainEvent.Notes,
            Status = "Pendiente"
        };

        _db.Contracts.Add(contract);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Asador seleccionado correctamente. Contrato creado." });
    }
}
