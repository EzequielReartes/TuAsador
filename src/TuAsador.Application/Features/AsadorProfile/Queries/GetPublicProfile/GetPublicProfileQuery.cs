using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.AsadorProfile.Queries.GetPublicProfile;

public record GetPublicProfileQuery(Guid AsadorProfileId) : IRequest<PublicAsadorProfileResponse?>;

public class PublicPortfolioImageItem
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
}

public class PublicAsadorProfileResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? PhotoUrl { get; init; }
    public string? WhatsApp { get; init; }
    public string? Instagram { get; init; }
    public string MainCity { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double AverageRating { get; init; }
    public double PunctualityRating { get; init; }
    public double PresenceRating { get; init; }
    public double PerformanceRating { get; init; }
    public double CancellationRate { get; init; }
    public List<string> SpecialtyNames { get; init; } = new();
    public List<PublicPortfolioImageItem> PortfolioImages { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public class GetPublicProfileQueryHandler : IRequestHandler<GetPublicProfileQuery, PublicAsadorProfileResponse?>
{
    private readonly IApplicationDbContext _db;

    public GetPublicProfileQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PublicAsadorProfileResponse?> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles
            .Include(p => p.User)
            .Include(p => p.Specialties)
            .Include(p => p.PortfolioImages)
            .FirstOrDefaultAsync(p => p.Id == request.AsadorProfileId, cancellationToken);

        if (profile == null) return null;

        var ratings = await _db.Ratings
            .Where(r => r.RevieweeId == profile.UserId)
            .ToListAsync(cancellationToken);

        var punctualityAvg = ratings.Count > 0 ? ratings.Average(r => r.PunctualityScore) : 0.0;
        var presenceAvg = ratings.Count > 0 ? ratings.Average(r => r.PresenceScore) : 0.0;
        var performanceAvg = ratings.Count > 0 ? ratings.Average(r => r.PerformanceScore) : 0.0;
        var overallAvg = ratings.Count > 0 ? (punctualityAvg + presenceAvg + performanceAvg) / 3.0 : 0.0;

        return new PublicAsadorProfileResponse
        {
            Id = profile.Id,
            Name = profile.User.Name,
            PhotoUrl = profile.User.ProfilePictureUrl
                ?? (profile.User.ProfilePictureData != null
                    ? $"/api/profile-picture?userId={profile.User.Id}"
                    : null)
                ?? profile.PhotoUrl,
            WhatsApp = profile.User.WhatsApp,
            Instagram = profile.Instagram,
            MainCity = profile.MainCity,
            Status = profile.Status,
            Description = profile.Description,
            AverageRating = Math.Round(overallAvg, 1),
            PunctualityRating = Math.Round(punctualityAvg, 1),
            PresenceRating = Math.Round(presenceAvg, 1),
            PerformanceRating = Math.Round(performanceAvg, 1),
            CancellationRate = profile.CancellationRate,
            SpecialtyNames = profile.Specialties.Select(s => s.Name).ToList(),
            PortfolioImages = profile.PortfolioImages
                .Where(pi => pi.IsApproved == true)
                .Select(pi => new PublicPortfolioImageItem
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageUrl
                })
                .ToList(),
            CreatedAt = profile.CreatedAt
        };
    }
}
