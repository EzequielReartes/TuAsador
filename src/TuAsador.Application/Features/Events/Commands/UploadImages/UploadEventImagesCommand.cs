using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Application.Common.Models;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Events.Commands.UploadImages;

public record UploadEventImagesCommand(Guid EventId, string UserId, List<FileData> Files) : IRequest<List<EventImageResult>>;

public class EventImageResult
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class UploadEventImagesCommandHandler : IRequestHandler<UploadEventImagesCommand, List<EventImageResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public UploadEventImagesCommandHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<List<EventImageResult>> Handle(UploadEventImagesCommand request, CancellationToken cancellationToken)
    {
        var domainEvent = await _db.Events
            .Include(e => e.Images)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (domainEvent == null)
            throw new KeyNotFoundException("Evento no encontrado");

        if (domainEvent.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el propietario de este evento");

        if (request.Files.Count == 0)
            throw new ArgumentException("Debe seleccionar al menos un archivo");

        var existingCount = domainEvent.Images.Count;
        var maxNew = 3 - existingCount;

        if (maxNew <= 0)
            throw new InvalidOperationException("El evento ya tiene el máximo de 3 imágenes");

        if (request.Files.Count > maxNew)
            throw new InvalidOperationException($"Solo puede agregar {maxNew} imagen(es) más. Máximo 3 por evento");

        var images = new List<EventImage>();

        foreach (var file in request.Files)
        {
            using var stream = new MemoryStream(file.Content);
            var imageUrl = await _fileStorage.SaveFileAsync(stream, file.FileName, "events", file.Length);

            images.Add(new EventImage
            {
                EventId = request.EventId,
                ImageUrl = imageUrl
            });
        }

        _db.EventImages.AddRange(images);
        await _db.SaveChangesAsync(cancellationToken);

        return images.Select(i => new EventImageResult
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            CreatedAt = i.CreatedAt
        }).ToList();
    }
}
