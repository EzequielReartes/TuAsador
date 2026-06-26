using MediatR;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Admin.Commands.ToggleUserActive;

public record ToggleUserActiveCommand(string UserId) : IRequest<ToggleUserActiveResult>;

public class ToggleUserActiveResult
{
    public string Message { get; init; } = string.Empty;
}

public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, ToggleUserActiveResult>
{
    private readonly IUserService _userService;

    public ToggleUserActiveCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ToggleUserActiveResult> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(request.UserId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        user.IsActive = !user.IsActive;
        await _userService.UpdateAsync(user);

        return new ToggleUserActiveResult
        {
            Message = user.IsActive ? "Usuario habilitado" : "Usuario deshabilitado"
        };
    }
}
