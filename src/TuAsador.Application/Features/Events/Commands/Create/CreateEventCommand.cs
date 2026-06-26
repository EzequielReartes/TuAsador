using MediatR;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Events.Commands.Create;

public record CreateEventCommand(
    string UserId,
    DateTime Date,
    TimeSpan Time,
    string City,
    string Address,
    int PeopleCount,
    string EventType,
    string? ServiceDesired,
    string? Notes
) : IRequest<CreateEventResult>;

public class CreateEventResult
{
    public Guid Id { get; init; }
    public string Message { get; init; } = "Evento creado correctamente";
}

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IApplicationDbContext _db;

    public CreateEventCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CreateEventResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var domainEvent = new TuAsador.Domain.Entities.Event
        {
            ClientId = request.UserId,
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
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateEventResult { Id = domainEvent.Id };
    }
}
