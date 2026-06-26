using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Events.Commands.Apply;

public record ApplyToEventCommand(Guid EventId, string UserId) : IRequest<ApplyToEventResult>;

public class ApplyToEventResult
{
    public string Message { get; init; } = "Postulación enviada correctamente";
}

public class ApplyToEventCommandHandler : IRequestHandler<ApplyToEventCommand, ApplyToEventResult>
{
    private readonly IApplicationDbContext _db;

    public ApplyToEventCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApplyToEventResult> Handle(ApplyToEventCommand request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
        if (profile == null)
            throw new InvalidOperationException("Perfil de asador no encontrado");

        var domainEvent = await _db.Events
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (domainEvent == null)
            throw new KeyNotFoundException("Evento no encontrado");

        if (domainEvent.Status != "Disponible")
            throw new InvalidOperationException("El evento ya no está disponible");

        if (domainEvent.Applications.Any(a => a.AsadorProfileId == profile.Id))
            throw new InvalidOperationException("Ya te has postulado a este evento");

        var application = new EventApplication
        {
            EventId = request.EventId,
            AsadorProfileId = profile.Id
        };

        _db.EventApplications.Add(application);
        await _db.SaveChangesAsync(cancellationToken);

        return new ApplyToEventResult();
    }
}
