using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TuAsador.Infrastructure;

public partial class TemporaryDbContext : DbContext
{
    public TemporaryDbContext()
    {
    }

    public TemporaryDbContext(DbContextOptions<TemporaryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsadorProfile> AsadorProfiles { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventApplication> EventApplications { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PortfolioImage> PortfolioImages { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=NOTEBOOKEZE\\LOCALSERVER;Database=TuAsadorDev;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsadorProfile>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AsadorProfiles_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.AsadorProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Specialties).WithMany(p => p.Asadores)
                .UsingEntity<Dictionary<string, object>>(
                    "AsadorSpecialty",
                    r => r.HasOne<Specialty>().WithMany().HasForeignKey("SpecialtiesId"),
                    l => l.HasOne<AsadorProfile>().WithMany().HasForeignKey("AsadoresId"),
                    j =>
                    {
                        j.HasKey("AsadoresId", "SpecialtiesId");
                        j.ToTable("AsadorSpecialties");
                        j.HasIndex(new[] { "SpecialtiesId" }, "IX_AsadorSpecialties_SpecialtiesId");
                    });
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasIndex(e => e.AsadorProfileId, "IX_Availabilities_AsadorProfileId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.AsadorProfile).WithMany(p => p.Availabilities).HasForeignKey(d => d.AsadorProfileId);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(e => e.AsadorProfileId, "IX_Contracts_AsadorProfileId");

            entity.HasIndex(e => e.ClientId, "IX_Contracts_ClientId");

            entity.HasIndex(e => e.EventId, "IX_Contracts_EventId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.AsadorProfile).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.AsadorProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Event).WithMany(p => p.Contracts).HasForeignKey(d => d.EventId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_Events_ClientId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Client).WithMany(p => p.Events)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EventApplication>(entity =>
        {
            entity.HasIndex(e => e.AsadorProfileId, "IX_EventApplications_AsadorProfileId");

            entity.HasIndex(e => e.EventId, "IX_EventApplications_EventId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.AsadorProfile).WithMany(p => p.EventApplications)
                .HasForeignKey(d => d.AsadorProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Event).WithMany(p => p.EventApplications)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PortfolioImage>(entity =>
        {
            entity.HasIndex(e => e.AsadorProfileId, "IX_PortfolioImages_AsadorProfileId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.AsadorProfile).WithMany(p => p.PortfolioImages).HasForeignKey(d => d.AsadorProfileId);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasIndex(e => e.ContractId, "IX_Ratings_ContractId");

            entity.HasIndex(e => e.RevieweeId, "IX_Ratings_RevieweeId");

            entity.HasIndex(e => e.ReviewerId, "IX_Ratings_ReviewerId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Contract).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Reviewee).WithMany(p => p.RatingReviewees)
                .HasForeignKey(d => d.RevieweeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.RatingReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
