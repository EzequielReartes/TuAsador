using MediatR;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Admin.Commands.ApprovePortfolioImage;

public record ApprovePortfolioImageCommand(Guid ImageId) : IRequest;

public class ApprovePortfolioImageCommandHandler : IRequestHandler<ApprovePortfolioImageCommand>
{
    private readonly IApplicationDbContext _db;

    public ApprovePortfolioImageCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(ApprovePortfolioImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _db.PortfolioImages.FindAsync([request.ImageId], cancellationToken);
        if (image == null)
            throw new KeyNotFoundException("Imagen no encontrada");

        image.IsApproved = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
