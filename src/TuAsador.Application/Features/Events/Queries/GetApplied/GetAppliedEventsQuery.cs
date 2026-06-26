using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Events.Queries.GetApplied;

public record GetAppliedEventsQuery(string UserId) : IRequest<List<EventDto>>;

public class GetAppliedEventsQueryHandler : IRequestHandler<GetAppliedEventsQuery, List<EventDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAppliedEventsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventDto>> Handle(GetAppliedEventsQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
        if (profile == null)
            return new List<EventDto>();

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
            .ToListAsync(cancellationToken);

        return events;
    }
}
