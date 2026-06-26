using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Specialties.Queries.GetAll;

public record GetAllSpecialtiesQuery : IRequest<List<SpecialtyDto>>;

public class SpecialtyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class GetAllSpecialtiesQueryHandler : IRequestHandler<GetAllSpecialtiesQuery, List<SpecialtyDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAllSpecialtiesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SpecialtyDto>> Handle(GetAllSpecialtiesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Specialties
            .Select(s => new SpecialtyDto { Id = s.Id, Name = s.Name })
            .ToListAsync(cancellationToken);
    }
}
