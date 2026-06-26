using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Admin.Queries.GetPortfolioImages;

public record GetPortfolioImagesQuery(bool PendingOnly) : IRequest<List<PortfolioImageDto>>;

public class GetPortfolioImagesQueryHandler : IRequestHandler<GetPortfolioImagesQuery, List<PortfolioImageDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPortfolioImagesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PortfolioImageDto>> Handle(GetPortfolioImagesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.PortfolioImages
            .Include(p => p.AsadorProfile)
            .ThenInclude(a => a.User)
            .AsQueryable();

        if (request.PendingOnly)
            query = query.Where(p => p.IsApproved == null);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PortfolioImageDto
            {
                Id = p.Id,
                AsadorProfileId = p.AsadorProfileId,
                AsadorName = p.AsadorProfile.User.Name,
                ImageUrl = p.ImageUrl,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
