namespace TuAsador.Application.DTOs;

public class AdminUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? WhatsApp { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PortfolioImageDto
{
    public Guid Id { get; set; }
    public Guid AsadorProfileId { get; set; }
    public string AsadorName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool? IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
