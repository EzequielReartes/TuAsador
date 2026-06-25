namespace TuAsador.Domain.Entities;

public class PortfolioImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AsadorProfileId { get; set; }
    public AsadorProfile AsadorProfile { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public bool? IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
