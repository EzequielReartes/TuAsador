using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Events.Queries.GetAll;

public record GetAllEventsQuery(string? UserId) : IRequest<List<EventDto>>;

public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, List<EventDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAllEventsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var profile = request.UserId != null
            ? await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken)
            : null;

        var query = _db.Events
            .Include(e => e.Client)
            .Include(e => e.Applications)
            .Include(e => e.Images)
            .Where(e => e.Status == "Disponible");

        if (profile != null)
            query = query.Where(e => !e.Applications.Any(a => a.AsadorProfileId == profile.Id));

        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(e => new EventDto
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
    }
}
