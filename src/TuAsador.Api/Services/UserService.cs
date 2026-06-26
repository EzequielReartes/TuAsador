using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Domain.Entities;

namespace TuAsador.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IQueryable<User> Users => _userManager.Users;

    public Task<User?> FindByIdAsync(string userId)
    {
        return _userManager.FindByIdAsync(userId);
    }

    public Task UpdateAsync(User user)
    {
        return _userManager.UpdateAsync(user);
    }
}
