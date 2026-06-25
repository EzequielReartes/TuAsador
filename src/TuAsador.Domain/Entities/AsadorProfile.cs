namespace TuAsador.Domain.Entities;

public class AsadorProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string? Description { get; set; }
    public string? Instagram { get; set; }
    public string? PhotoUrl { get; set; }
    public string MainCity { get; set; } = string.Empty;
    public string Status { get; set; } = "Pendiente";
    public double CancellationRate { get; set; }
    public double AverageRating { get; set; }
    public double PunctualityRating { get; set; }
    public double PresenceRating { get; set; }
    public double PerformanceRating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PortfolioImage> PortfolioImages { get; set; } = new List<PortfolioImage>();
    public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
}
