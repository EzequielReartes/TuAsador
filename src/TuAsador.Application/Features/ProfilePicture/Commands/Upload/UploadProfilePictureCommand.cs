using MediatR;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.ProfilePicture.Commands.Upload;

public record UploadProfilePictureCommand(string UserId, byte[] FileData, string ContentType, string FileName) : IRequest<UploadProfilePictureResult>;

public class UploadProfilePictureResult
{
    public string ProfilePictureUrl { get; init; } = string.Empty;
}

public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, UploadProfilePictureResult>
{
    private readonly IUserService _userService;

    public UploadProfilePictureCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UploadProfilePictureResult> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(request.UserId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        user.ProfilePictureData = request.FileData;
        user.ProfilePictureContentType = request.ContentType;
        user.ProfilePictureUrl = null;

        await _userService.UpdateAsync(user);

        return new UploadProfilePictureResult();
    }
}
