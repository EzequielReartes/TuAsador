using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Portfolio.Queries.GetMyImages;

public record GetMyPortfolioImagesQuery(string UserId) : IRequest<List<PortfolioImageItem>>;

public class PortfolioImageItem
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool? IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class GetMyPortfolioImagesQueryHandler : IRequestHandler<GetMyPortfolioImagesQuery, List<PortfolioImageItem>>
{
    private readonly IApplicationDbContext _db;

    public GetMyPortfolioImagesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PortfolioImageItem>> Handle(GetMyPortfolioImagesQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
            return new List<PortfolioImageItem>();

        return await _db.PortfolioImages
            .Where(p => p.AsadorProfileId == profile.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PortfolioImageItem
            {
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
