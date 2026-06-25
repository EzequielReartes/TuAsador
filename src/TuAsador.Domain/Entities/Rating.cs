namespace TuAsador.Domain.Entities;

public class Rating
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
    public string ReviewerId { get; set; } = string.Empty;
    public User Reviewer { get; set; } = null!;
    public string RevieweeId { get; set; } = string.Empty;
    public User Reviewee { get; set; } = null!;
    public int PunctualityScore { get; set; }
    public int PresenceScore { get; set; }
    public int PerformanceScore { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
