using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class AsadorProfile
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? Description { get; set; }

    public string? Instagram { get; set; }

    public string? PhotoUrl { get; set; }

    public string MainCity { get; set; } = null!;

    public string Status { get; set; } = null!;

    public double CancellationRate { get; set; }

    public double AverageRating { get; set; }

    public double PunctualityRating { get; set; }

    public double PresenceRating { get; set; }

    public double PerformanceRating { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<EventApplication> EventApplications { get; set; } = new List<EventApplication>();

    public virtual ICollection<PortfolioImage> PortfolioImages { get; set; } = new List<PortfolioImage>();

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}
