using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Users.Queries.GetAll;

public record GetAllUsersQuery : IRequest<List<UserListItem>>;

public class UserListItem
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? UserName { get; init; }
    public string Role { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? WhatsApp { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserListItem>>
{
    private readonly IUserService _userService;

    public GetAllUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<List<UserListItem>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userService.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserListItem
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                UserName = u.UserName,
                Role = u.Role,
                PhoneNumber = u.PhoneNumber,
                WhatsApp = u.WhatsApp,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
