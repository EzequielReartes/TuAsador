using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Events.Queries.GetById;

public record GetEventByIdQuery(Guid Id, string? UserId) : IRequest<EventDetailDto?>;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDetailDto?>
{
    private readonly IApplicationDbContext _db;

    public GetEventByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<EventDetailDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = request.UserId != null
            ? await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken)
            : null;

        var domainEvent = await _db.Events
            .Include(e => e.Client)
            .Include(e => e.Images)
            .Include(e => e.Applications)
                .ThenInclude(a => a.AsadorProfile)
                    .ThenInclude(ap => ap.User)
            .Include(e => e.Applications)
                .ThenInclude(a => a.AsadorProfile)
                    .ThenInclude(ap => ap.Specialties)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (domainEvent == null) return null;

        Contract? contract = null;
        if (request.UserId == domainEvent.ClientId && domainEvent.Status == "Asignado")
        {
            contract = await _db.Contracts
                .Where(c => c.EventId == domainEvent.Id && c.ClientId == request.UserId)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var isOwner = request.UserId == domainEvent.ClientId;

        Dictionary<string, double> ratingCache = new();
        if (isOwner && domainEvent.Applications.Count > 0)
        {
            var userIds = domainEvent.Applications.Select(a => a.AsadorProfile.UserId).Distinct().ToList();
            var ratingsByUser = await _db.Ratings
                .Where(r => userIds.Contains(r.RevieweeId))
                .GroupBy(r => r.RevieweeId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Avg = (g.Average(r => r.PunctualityScore) + g.Average(r => r.PresenceScore) + g.Average(r => r.PerformanceScore)) / 3.0
                })
                .ToListAsync(cancellationToken);

            ratingCache = ratingsByUser.ToDictionary(x => x.UserId, x => Math.Round(x.Avg, 1));
        }

        return new EventDetailDto
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
            ContractId = contract?.Id,
            ContractStatus = contract?.Status,
            Applications = isOwner
                ? domainEvent.Applications.Select(a => new EventApplicationDto
                {
                    Id = a.Id,
                    AsadorProfileId = a.AsadorProfileId,
                    AsadorName = a.AsadorProfile.User.Name,
                    AsadorPhotoUrl = a.AsadorProfile.User.ProfilePictureUrl
                        ?? (a.AsadorProfile.User.ProfilePictureData != null
                            ? $"/api/profile-picture?userId={a.AsadorProfile.User.Id}"
                            : null)
                        ?? a.AsadorProfile.PhotoUrl,
                    WhatsApp = a.AsadorProfile.User.WhatsApp,
                    MainCity = a.AsadorProfile.MainCity,
                    AverageRating = ratingCache.GetValueOrDefault(a.AsadorProfile.UserId, 0),
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    Description = a.AsadorProfile.Description,
                    SpecialtyNames = a.AsadorProfile.Specialties.Select(s => s.Name).ToList()
                }).ToList()
                : new List<EventApplicationDto>()
        };
    }
}
