namespace TuAsador.Application.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ServiceDesired { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ApplicationCount { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public string? ApplicationStatus { get; set; }
}

public class EventDetailDto
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PeopleCount { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ServiceDesired { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<EventApplicationDto> Applications { get; set; } = new();
    public bool HasApplied { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}

public class EventApplicationDto
{
    public Guid Id { get; set; }
    public Guid AsadorProfileId { get; set; }
    public string AsadorName { get; set; } = string.Empty;
    public string? AsadorPhotoUrl { get; set; }
    public string? WhatsApp { get; set; }
    public string MainCity { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    public List<string> SpecialtyNames { get; set; } = new();
}

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
