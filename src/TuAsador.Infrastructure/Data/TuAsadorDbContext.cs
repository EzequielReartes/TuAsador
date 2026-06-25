using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuAsador.Domain.Entities;

namespace TuAsador.Infrastructure.Data;

public class TuAsadorDbContext : IdentityDbContext<User>
{
    public TuAsadorDbContext(DbContextOptions<TuAsadorDbContext> options) : base(options) { }

    public DbSet<AsadorProfile> AsadorProfiles => Set<AsadorProfile>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<PortfolioImage> PortfolioImages => Set<PortfolioImage>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<EventImage> EventImages => Set<EventImage>();
    public DbSet<EventApplication> EventApplications => Set<EventApplication>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AsadorProfile>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Event>()
            .HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany()
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Contract>()
            .HasOne(c => c.AsadorProfile)
            .WithMany()
            .HasForeignKey(c => c.AsadorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Contract>()
            .HasOne(c => c.Event)
            .WithMany()
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EventApplication>()
            .HasOne(ea => ea.Event)
            .WithMany(e => e.Applications)
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EventApplication>()
            .HasOne(ea => ea.AsadorProfile)
            .WithMany()
            .HasForeignKey(ea => ea.AsadorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rating>()
            .HasOne(r => r.Contract)
            .WithMany()
            .HasForeignKey(r => r.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rating>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rating>()
            .HasOne(r => r.Reviewee)
            .WithMany()
            .HasForeignKey(r => r.RevieweeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PortfolioImage>()
            .HasOne(p => p.AsadorProfile)
            .WithMany(a => a.PortfolioImages)
            .HasForeignKey(p => p.AsadorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EventImage>()
            .HasOne(ei => ei.Event)
            .WithMany(e => e.Images)
            .HasForeignKey(ei => ei.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Availability>()
            .HasOne(a => a.AsadorProfile)
            .WithMany(ap => ap.Availabilities)
            .HasForeignKey(a => a.AsadorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Specialty>()
            .HasMany(s => s.Asadores)
            .WithMany(a => a.Specialties)
            .UsingEntity(j => j.ToTable("AsadorSpecialties"));
    }
}
