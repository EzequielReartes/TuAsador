using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Events.Queries.GetApplications;

public record GetEventApplicationsQuery(Guid EventId, string UserId) : IRequest<List<EventApplicationDto>>;

public class GetEventApplicationsQueryHandler : IRequestHandler<GetEventApplicationsQuery, List<EventApplicationDto>>
{
    private readonly IApplicationDbContext _db;

    public GetEventApplicationsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventApplicationDto>> Handle(GetEventApplicationsQuery request, CancellationToken cancellationToken)
    {
        var domainEvent = await _db.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (domainEvent == null)
            throw new KeyNotFoundException("Evento no encontrado");

        if (domainEvent.ClientId != request.UserId)
            throw new UnauthorizedAccessException();

        var applications = await _db.EventApplications
            .Include(a => a.AsadorProfile)
                .ThenInclude(ap => ap.User)
            .Include(a => a.AsadorProfile)
                .ThenInclude(ap => ap.Specialties)
            .Where(a => a.EventId == request.EventId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return applications.Select(a => new EventApplicationDto
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
    }
}
