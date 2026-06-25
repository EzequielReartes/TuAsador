namespace TuAsador.Domain.Entities;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientId { get; set; } = string.Empty;
    public User Client { get; set; } = null!;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ServiceDesired { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Disponible";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
    public ICollection<EventImage> Images { get; set; } = new List<EventImage>();
}
