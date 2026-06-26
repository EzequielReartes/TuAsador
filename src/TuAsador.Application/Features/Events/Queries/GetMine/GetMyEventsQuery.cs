using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Events.Queries.GetMine;

public record GetMyEventsQuery(string UserId) : IRequest<List<EventDto>>;

public class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, List<EventDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMyEventsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventDto>> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _db.Events
            .Include(e => e.Client)
            .Include(e => e.Applications)
            .Include(e => e.Images)
            .Where(e => e.ClientId == request.UserId)
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
