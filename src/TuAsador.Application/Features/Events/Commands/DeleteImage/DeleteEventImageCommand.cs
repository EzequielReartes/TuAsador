using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Events.Commands.DeleteImage;

public record DeleteEventImageCommand(Guid EventId, Guid ImageId, string UserId) : IRequest;

public class DeleteEventImageCommandHandler : IRequestHandler<DeleteEventImageCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public DeleteEventImageCommandHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeleteEventImageCommand request, CancellationToken cancellationToken)
    {
        var domainEvent = await _db.Events.FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (domainEvent == null)
            throw new KeyNotFoundException("Evento no encontrado");

        if (domainEvent.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el propietario de este evento");

        var image = await _db.EventImages
            .FirstOrDefaultAsync(i => i.Id == request.ImageId && i.EventId == request.EventId, cancellationToken);

        if (image == null)
            throw new KeyNotFoundException("Imagen no encontrada");

        await _fileStorage.DeleteFileAsync(image.ImageUrl);

        _db.EventImages.Remove(image);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
