using TuAsador.Application.Features.Auth;

namespace TuAsador.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string email, string password);
    Task<AuthResponse> RegisterAsync(string name, string email, string password, string? whatsApp, string role);
}
