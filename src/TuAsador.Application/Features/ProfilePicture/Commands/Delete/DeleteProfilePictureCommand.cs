using MediatR;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.ProfilePicture.Commands.Delete;

public record DeleteProfilePictureCommand(string UserId) : IRequest;

public class DeleteProfilePictureCommandHandler : IRequestHandler<DeleteProfilePictureCommand>
{
    private readonly IUserService _userService;

    public DeleteProfilePictureCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Handle(DeleteProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(request.UserId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        user.ProfilePictureData = null;
        user.ProfilePictureContentType = null;
        user.ProfilePictureUrl = null;

        await _userService.UpdateAsync(user);
    }
}
