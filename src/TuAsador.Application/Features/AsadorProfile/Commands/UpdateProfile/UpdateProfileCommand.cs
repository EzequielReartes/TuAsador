using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.AsadorProfile.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string UserId,
    string? Description,
    string? Instagram,
    string? PhotoUrl,
    string MainCity,
    string? WhatsApp,
    List<Guid> SpecialtyIds
) : IRequest;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _db;

    public UpdateProfileCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _db.AsadorProfiles
            .Include(p => p.User)
            .Include(p => p.Specialties)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
            throw new KeyNotFoundException("Perfil de asador no encontrado");

        profile.Description = request.Description;
        profile.Instagram = request.Instagram;
        profile.PhotoUrl = request.PhotoUrl;
        profile.MainCity = request.MainCity;

        if (request.WhatsApp != null)
            profile.User.WhatsApp = request.WhatsApp;

        if (request.SpecialtyIds.Count != 0)
        {
            var specialties = await _db.Specialties
                .Where(s => request.SpecialtyIds.Contains(s.Id))
                .ToListAsync(cancellationToken);
            profile.Specialties.Clear();
            foreach (var s in specialties)
                profile.Specialties.Add(s);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
