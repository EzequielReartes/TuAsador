using Microsoft.EntityFrameworkCore;
using TuAsador.Domain.Entities;

namespace TuAsador.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AsadorProfile> AsadorProfiles { get; }
    DbSet<Specialty> Specialties { get; }
    DbSet<PortfolioImage> PortfolioImages { get; }
    DbSet<Availability> Availabilities { get; }
    DbSet<Event> Events { get; }
    DbSet<Contract> Contracts { get; }
    DbSet<EventImage> EventImages { get; }
    DbSet<EventApplication> EventApplications { get; }
    DbSet<Rating> Ratings { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
