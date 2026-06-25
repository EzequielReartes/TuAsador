namespace TuAsador.Domain.Entities;

public class EventApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Guid AsadorProfileId { get; set; }
    public AsadorProfile AsadorProfile { get; set; } = null!;
    public string Status { get; set; } = "Pendiente";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
