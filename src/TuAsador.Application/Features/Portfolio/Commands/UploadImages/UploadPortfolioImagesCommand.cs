using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Application.Common.Models;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Portfolio.Commands.UploadImages;

public record UploadPortfolioImagesCommand(string UserId, List<FileData> Files) : IRequest<List<PortfolioImageItem>>;

public class PortfolioImageItem
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool? IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class UploadPortfolioImagesCommandHandler : IRequestHandler<UploadPortfolioImagesCommand, List<PortfolioImageItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public UploadPortfolioImagesCommandHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<List<PortfolioImageItem>> Handle(UploadPortfolioImagesCommand request, CancellationToken cancellationToken)
    {
        if (request.Files.Count == 0)
            throw new ArgumentException("Debe seleccionar al menos un archivo");

        if (request.Files.Count > 5)
            throw new ArgumentException("Máximo 5 imágenes por subida");

        var profile = await _db.AsadorProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
            throw new KeyNotFoundException("Perfil de asador no encontrado");

        var images = new List<PortfolioImage>();

        foreach (var file in request.Files)
        {
            using var stream = new MemoryStream(file.Content);
            var imageUrl = await _fileStorage.SaveFileAsync(stream, file.FileName, "portfolio", file.Length);

            images.Add(new PortfolioImage
            {
                AsadorProfileId = profile.Id,
                ImageUrl = imageUrl,
                IsApproved = null
            });
        }

        _db.PortfolioImages.AddRange(images);
        await _db.SaveChangesAsync(cancellationToken);

        return images.Select(i => new PortfolioImageItem
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            IsApproved = i.IsApproved,
            CreatedAt = i.CreatedAt
        }).ToList();
    }
}
