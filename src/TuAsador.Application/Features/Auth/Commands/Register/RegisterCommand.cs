using MediatR;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Application.Features.Auth;

namespace TuAsador.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string? WhatsApp,
    string Role
) : IRequest<AuthResponse>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(
            request.Name, request.Email, request.Password,
            request.WhatsApp, request.Role);
    }
}
