namespace TuAsador.Application.Features.Admin;

public class AdminUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? WhatsApp { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class PortfolioImageDto
{
    public Guid Id { get; init; }
    public Guid AsadorProfileId { get; init; }
    public string AsadorName { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public bool? IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}
