namespace TuAsador.Application.Features.Contracts;

public class ContractDto
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public Guid EventId { get; init; }
    public DateTime EventDate { get; init; }
    public TimeSpan EventTime { get; init; }
    public string EventCity { get; init; } = string.Empty;
    public string EventAddress { get; init; } = string.Empty;
    public int EventPeopleCount { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string? EventServiceDesired { get; init; }
    public string? EventNotes { get; init; }
    public List<string> EventImageUrls { get; init; } = new();

    public Guid AsadorProfileId { get; init; }
    public string AsadorName { get; init; } = string.Empty;
    public string? AsadorPhotoUrl { get; init; }
    public string? AsadorWhatsApp { get; init; }
    public string AsadorMainCity { get; init; } = string.Empty;
    public double AsadorAverageRating { get; init; }
    public List<string> AsadorSpecialtyNames { get; init; } = new();
}
