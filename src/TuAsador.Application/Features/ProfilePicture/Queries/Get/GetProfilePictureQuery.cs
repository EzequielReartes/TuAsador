using MediatR;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Features.ProfilePicture.Queries.Get;

public record GetProfilePictureQuery(string? UserId) : IRequest<ProfilePictureResult?>;

public class ProfilePictureResult
{
    public byte[] Data { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
}

public class GetProfilePictureQueryHandler : IRequestHandler<GetProfilePictureQuery, ProfilePictureResult?>
{
    private readonly IUserService _userService;

    public GetProfilePictureQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ProfilePictureResult?> Handle(GetProfilePictureQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return null;

        var user = await _userService.FindByIdAsync(request.UserId);
        if (user?.ProfilePictureData == null || user.ProfilePictureContentType == null)
            return null;

        return new ProfilePictureResult
        {
            Data = user.ProfilePictureData,
            ContentType = user.ProfilePictureContentType
        };
    }
}
