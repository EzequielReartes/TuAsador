using Microsoft.AspNetCore.Identity;

namespace TuAsador.Domain.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string Role { get; set; } = "Cliente";
    public bool IsActive { get; set; } = true;
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
