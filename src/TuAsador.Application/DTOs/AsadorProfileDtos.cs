namespace TuAsador.Application.DTOs;

public class AsadorProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string? Description { get; set; }
    public string? Instagram { get; set; }
    public string? PhotoUrl { get; set; }
    public string MainCity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<Guid> SpecialtyIds { get; set; } = new();
    public List<string> SpecialtyNames { get; set; } = new();
}

public class UpdateAsadorProfileRequest
{
    public string? Description { get; set; }
    public string? Instagram { get; set; }
    public string? PhotoUrl { get; set; }
    public string MainCity { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public List<Guid> SpecialtyIds { get; set; } = new();
}
