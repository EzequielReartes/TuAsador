using MediatR;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Admin.Commands.RejectPortfolioImage;

public record RejectPortfolioImageCommand(Guid ImageId) : IRequest;

public class RejectPortfolioImageCommandHandler : IRequestHandler<RejectPortfolioImageCommand>
{
    private readonly IApplicationDbContext _db;

    public RejectPortfolioImageCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(RejectPortfolioImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _db.PortfolioImages.FindAsync([request.ImageId], cancellationToken);
        if (image == null)
            throw new KeyNotFoundException("Imagen no encontrada");

        image.IsApproved = false;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
