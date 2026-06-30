using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Contracts.Queries.GetMyContracts;

public record GetMyContractsQuery(string UserId) : IRequest<List<ContractDto>>;

public class GetMyContractsQueryHandler : IRequestHandler<GetMyContractsQuery, List<ContractDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMyContractsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ContractDto>> Handle(GetMyContractsQuery request, CancellationToken cancellationToken)
    {
        var contracts = await _db.Contracts
            .Include(c => c.Event).ThenInclude(e => e.Images)
            .Include(c => c.AsadorProfile).ThenInclude(ap => ap.User)
            .Include(c => c.AsadorProfile).ThenInclude(ap => ap.Specialties)
            .Where(c => c.ClientId == request.UserId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return contracts.Where(c => c.Event != null).Select(c => new ContractDto
        {
            Id = c.Id,
            Status = c.Status,
            CreatedAt = c.CreatedAt,

            EventId = c.Event!.Id,
            EventDate = c.Event.Date,
            EventTime = c.Event.Time,
            EventCity = c.Event.City,
            EventAddress = c.Event.Address,
            EventPeopleCount = c.Event.PeopleCount,
            EventType = c.Event.EventType,
            EventServiceDesired = c.Event.ServiceDesired,
            EventNotes = c.Event.Notes,
            EventImageUrls = c.Event.Images.Select(i => i.ImageUrl).ToList(),

            AsadorProfileId = c.AsadorProfileId,
            AsadorName = c.AsadorProfile.User.Name,
            AsadorPhotoUrl = c.AsadorProfile.User.ProfilePictureUrl
                ?? (c.AsadorProfile.User.ProfilePictureData != null
                    ? $"/api/profile-picture?userId={c.AsadorProfile.User.Id}"
                    : null)
                ?? c.AsadorProfile.PhotoUrl,
            AsadorWhatsApp = c.AsadorProfile.User.WhatsApp,
            AsadorMainCity = c.AsadorProfile.MainCity,
            AsadorAverageRating = c.AsadorProfile.AverageRating,
            AsadorSpecialtyNames = c.AsadorProfile.Specialties.Select(s => s.Name).ToList()
        }).ToList();
    }
}
