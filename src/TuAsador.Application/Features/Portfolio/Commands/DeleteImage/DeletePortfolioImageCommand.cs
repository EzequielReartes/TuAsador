using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Portfolio.Commands.DeleteImage;

public record DeletePortfolioImageCommand(Guid ImageId, string UserId) : IRequest;

public class DeletePortfolioImageCommandHandler : IRequestHandler<DeletePortfolioImageCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public DeletePortfolioImageCommandHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeletePortfolioImageCommand request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
            throw new KeyNotFoundException("Perfil de asador no encontrado");

        var image = await _db.PortfolioImages
            .FirstOrDefaultAsync(p => p.Id == request.ImageId && p.AsadorProfileId == profile.Id, cancellationToken);

        if (image == null)
            throw new KeyNotFoundException("Imagen no encontrada");

        await _fileStorage.DeleteFileAsync(image.ImageUrl);

        _db.PortfolioImages.Remove(image);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
