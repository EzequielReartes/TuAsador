using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.Ratings.Commands.CreateRating;

public record CreateRatingCommand(Guid ContractId, string UserId, int PunctualityScore, int PresenceScore, int PerformanceScore, string? Comment)
    : IRequest<CreateRatingResult>;

public class CreateRatingResult
{
    public string Message { get; init; } = "Valoración enviada correctamente";
}

public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, CreateRatingResult>
{
    private readonly IApplicationDbContext _db;

    public CreateRatingCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CreateRatingResult> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
    {
        var contract = await _db.Contracts
            .Include(c => c.AsadorProfile).ThenInclude(ap => ap.User)
            .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

        if (contract == null)
            throw new KeyNotFoundException("Contrato no encontrado");

        if (contract.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el dueño de este contrato");

        if (contract.Status != "Finalizado")
            throw new InvalidOperationException("Solo se pueden valorar contratos finalizados");

        var existing = await _db.Ratings.AnyAsync(r => r.ContractId == request.ContractId, cancellationToken);
        if (existing)
            throw new InvalidOperationException("Este contrato ya fue valorado");

        var rating = new Rating
        {
            ContractId = request.ContractId,
            ReviewerId = request.UserId,
            RevieweeId = contract.AsadorProfile.UserId,
            PunctualityScore = request.PunctualityScore,
            PresenceScore = request.PresenceScore,
            PerformanceScore = request.PerformanceScore,
            Comment = request.Comment
        };

        _db.Ratings.Add(rating);

        // Recalculate asador averages
        var profile = contract.AsadorProfile;
        var allRatings = await _db.Ratings
            .Where(r => r.RevieweeId == profile.UserId)
            .ToListAsync(cancellationToken);

        profile.AverageRating = allRatings.Any()
            ? Math.Round(allRatings.Average(r => (r.PunctualityScore + r.PresenceScore + r.PerformanceScore) / 3.0), 1)
            : 0;
        profile.PunctualityRating = allRatings.Any()
            ? Math.Round(allRatings.Average(r => r.PunctualityScore), 1)
            : 0;
        profile.PresenceRating = allRatings.Any()
            ? Math.Round(allRatings.Average(r => r.PresenceScore), 1)
            : 0;
        profile.PerformanceRating = allRatings.Any()
            ? Math.Round(allRatings.Average(r => r.PerformanceScore), 1)
            : 0;

        await _db.SaveChangesAsync(cancellationToken);

        return new CreateRatingResult();
    }
}
