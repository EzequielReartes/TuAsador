using TuAsador.Domain.Entities;

namespace TuAsador.Application.Common.Interfaces;

public interface IUserService
{
    IQueryable<User> Users { get; }
    Task<User?> FindByIdAsync(string userId);
    Task UpdateAsync(User user);
}
