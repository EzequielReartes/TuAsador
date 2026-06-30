using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.AsadorProfile.Queries.GetMyProfile;

public record GetMyProfileQuery(string UserId) : IRequest<AsadorProfileResponse?>;

public class AsadorProfileResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? WhatsApp { get; init; }
    public string? Description { get; init; }
    public string? Instagram { get; init; }
    public string? PhotoUrl { get; init; }
    public string MainCity { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public List<Guid> SpecialtyIds { get; init; } = new();
    public List<string> SpecialtyNames { get; init; } = new();
}

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, AsadorProfileResponse?>
{
    private readonly IApplicationDbContext _db;

    public GetMyProfileQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AsadorProfileResponse?> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles
            .Include(p => p.User)
            .Include(p => p.Specialties)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null) return null;

        return new AsadorProfileResponse
        {
            Id = profile.Id,
            Name = profile.User.Name,
            Email = profile.User.Email!,
            WhatsApp = profile.User.WhatsApp,
            Description = profile.Description,
            Instagram = profile.Instagram,
            PhotoUrl = profile.User.ProfilePictureUrl
                ?? (profile.User.ProfilePictureData != null
                    ? $"/api/profile-picture?userId={profile.User.Id}"
                    : null)
                ?? profile.PhotoUrl,
            MainCity = profile.MainCity,
            Status = profile.Status,
            SpecialtyIds = profile.Specialties.Select(s => s.Id).ToList(),
            SpecialtyNames = profile.Specialties.Select(s => s.Name).ToList()
        };
    }
}

public class UpdateAsadorProfileRequest
{
    public string? Description { get; init; }
    public string? Instagram { get; init; }
    public string? PhotoUrl { get; init; }
    public string MainCity { get; init; } = string.Empty;
    public string? WhatsApp { get; init; }
    public List<Guid> SpecialtyIds { get; init; } = new();
}
