namespace TuAsador.Application.Features.Ratings;

public class CreateRatingRequest
{
    public Guid ContractId { get; set; }
    public int PunctualityScore { get; set; }
    public int PresenceScore { get; set; }
    public int PerformanceScore { get; set; }
    public string? Comment { get; set; }
}
