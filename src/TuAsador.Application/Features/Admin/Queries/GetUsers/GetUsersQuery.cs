using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Admin.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<AdminUserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<AdminUserDto>>
{
    private readonly IUserService _userService;

    public GetUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<List<AdminUserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userService.Users
            .OrderBy(u => u.Name)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email!,
                UserName = u.UserName!,
                Role = u.Role,
                PhoneNumber = u.PhoneNumber,
                WhatsApp = u.WhatsApp,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
