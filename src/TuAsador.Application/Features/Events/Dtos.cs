namespace TuAsador.Application.Features.Events;

public class CreateEventRequest
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ServiceDesired { get; set; }
    public string? Notes { get; set; }
}

public class EventDto
{
    public Guid Id { get; init; }
    public string ClientId { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public TimeSpan Time { get; init; }
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int PeopleCount { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string? ServiceDesired { get; init; }
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public int ApplicationCount { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    public string? ApplicationStatus { get; init; }
}

public class EventDetailDto
{
    public Guid Id { get; init; }
    public string ClientId { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public TimeSpan Time { get; init; }
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int PeopleCount { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string? ServiceDesired { get; init; }
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<EventApplicationDto> Applications { get; init; } = new();
    public bool HasApplied { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    public Guid? ContractId { get; init; }
    public string? ContractStatus { get; init; }
}

public class EventApplicationDto
{
    public Guid Id { get; init; }
    public Guid AsadorProfileId { get; init; }
    public string AsadorName { get; init; } = string.Empty;
    public string? AsadorPhotoUrl { get; init; }
    public string? WhatsApp { get; init; }
    public string MainCity { get; init; } = string.Empty;
    public double AverageRating { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public List<string> SpecialtyNames { get; init; } = new();
}
