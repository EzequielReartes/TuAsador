namespace TuAsador.Domain.Entities;

public class Contract
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientId { get; set; } = string.Empty;
    public User Client { get; set; } = null!;
    public Guid AsadorProfileId { get; set; }
    public AsadorProfile AsadorProfile { get; set; } = null!;
    public Guid? EventId { get; set; }
    public Event? Event { get; set; }
    public string Type { get; set; } = "Directa";
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ServiceDesired { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pendiente";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
