using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Events.Commands.SelectApplication;

public record SelectApplicationCommand(Guid EventId, Guid ApplicationId, string UserId) : IRequest<SelectApplicationResult>;

public class SelectApplicationResult
{
    public string Message { get; init; } = "Asador seleccionado correctamente. Contrato creado.";
}

public class SelectApplicationCommandHandler : IRequestHandler<SelectApplicationCommand, SelectApplicationResult>
{
    private readonly IApplicationDbContext _db;

    public SelectApplicationCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SelectApplicationResult> Handle(SelectApplicationCommand request, CancellationToken cancellationToken)
    {
        var domainEvent = await _db.Events
            .Include(e => e.Applications)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (domainEvent == null)
            throw new KeyNotFoundException("Evento no encontrado");

        if (domainEvent.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el propietario de este evento");

        if (domainEvent.Status != "Disponible")
            throw new InvalidOperationException("El evento ya no está disponible");

        var application = domainEvent.Applications
            .FirstOrDefault(a => a.Id == request.ApplicationId);

        if (application == null)
            throw new KeyNotFoundException("Postulación no encontrada");

        if (application.Status != "Pendiente")
            throw new InvalidOperationException("Esta postulación ya fue procesada");

        application.Status = "Aceptada";
        domainEvent.Status = "Asignado";

        foreach (var other in domainEvent.Applications.Where(a => a.Id != request.ApplicationId))
        {
            other.Status = "Rechazada";
        }

        var contract = new Contract
        {
            ClientId = request.UserId,
            AsadorProfileId = application.AsadorProfileId,
            EventId = request.EventId,
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
        await _db.SaveChangesAsync(cancellationToken);

        return new SelectApplicationResult();
    }
}
