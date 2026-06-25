using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class Rating
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public string ReviewerId { get; set; } = null!;

    public string RevieweeId { get; set; } = null!;

    public int PunctualityScore { get; set; }

    public int PresenceScore { get; set; }

    public int PerformanceScore { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual AspNetUser Reviewee { get; set; } = null!;

    public virtual AspNetUser Reviewer { get; set; } = null!;
}
