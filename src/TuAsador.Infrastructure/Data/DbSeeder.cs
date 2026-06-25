using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuAsador.Domain.Entities;

namespace TuAsador.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(TuAsadorDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (userManager.Users.Any()) return;

        string[] roles = ["Admin", "Asador", "Cliente"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var password = "TuAsador123!";

        var admin = new User
        {
            UserName = "admin",
            Email = "admin@tuasador.com",
            Name = "Admin TuAsador",
            Role = "Admin",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, password);
        await userManager.AddToRoleAsync(admin, "Admin");

        var asador1 = new User
        {
            UserName = "carlosgrill",
            Email = "carlos@email.com",
            Name = "Carlos El Parrillero",
            WhatsApp = "+5491112345678",
            Role = "Asador",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(asador1, password);
        await userManager.AddToRoleAsync(asador1, "Asador");

        var asador2 = new User
        {
            UserName = "martinfuego",
            Email = "martin@email.com",
            Name = "Martin Fuego",
            WhatsApp = "+5491123456789",
            Role = "Asador",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(asador2, password);
        await userManager.AddToRoleAsync(asador2, "Asador");

        var cliente1 = new User
        {
            UserName = "juanperez",
            Email = "juan@email.com",
            Name = "Juan Pérez",
            WhatsApp = "+5491134567890",
            Role = "Cliente",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(cliente1, password);
        await userManager.AddToRoleAsync(cliente1, "Cliente");

        var cliente2 = new User
        {
            UserName = "marialog",
            Email = "maria@email.com",
            Name = "María López",
            WhatsApp = "+5491145678901",
            Role = "Cliente",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(cliente2, password);
        await userManager.AddToRoleAsync(cliente2, "Cliente");

        if (!context.Specialties.Any())
        {
            var specialties = new[]
            {
                new Specialty { Name = "Asado" },
                new Specialty { Name = "Estaca" },
                new Specialty { Name = "Disco" },
                new Specialty { Name = "Parrilla" },
                new Specialty { Name = "Cocina al horno" },
            };
            context.Specialties.AddRange(specialties);
            await context.SaveChangesAsync();
        }

        if (!context.AsadorProfiles.Any())
        {
            context.AsadorProfiles.AddRange(
                new AsadorProfile
                {
                    UserId = asador1.Id,
                    MainCity = "Buenos Aires",
                    Description = "Asador profesional con 10 años de experiencia en todo tipo de eventos.",
                    Status = "Verificado",
                    Instagram = "@carlosgrill"
                },
                new AsadorProfile
                {
                    UserId = asador2.Id,
                    MainCity = "Córdoba",
                    Description = "Especialista en asados tradicionales y cocina al disco.",
                    Status = "Pendiente",
                    Instagram = "@martinfuego"
                }
            );

            await context.SaveChangesAsync();
        }

        if (!context.PortfolioImages.Any())
        {
            var carlosProfile = await context.AsadorProfiles
                .FirstOrDefaultAsync(p => p.UserId == asador1.Id);
            var martinProfile = await context.AsadorProfiles
                .FirstOrDefaultAsync(p => p.UserId == asador2.Id);

            var images = new List<PortfolioImage>();

            if (carlosProfile != null)
            {
                images.AddRange([
                    new PortfolioImage
                    {
                        AsadorProfileId = carlosProfile.Id,
                        ImageUrl = "https://placehold.co/600x400/1a3a1a/46cf5d?text=Parrillada+Completa",
                        IsApproved = true
                    },
                    new PortfolioImage
                    {
                        AsadorProfileId = carlosProfile.Id,
                        ImageUrl = "https://placehold.co/600x400/3a2a1a/f59e0b?text=Asado+Pendiente",
                        IsApproved = null
                    },
                    new PortfolioImage
                    {
                        AsadorProfileId = carlosProfile.Id,
                        ImageUrl = "https://placehold.co/600x400/991b1b/ffffff?text=Bondiola+Rechazada",
                        IsApproved = false
                    }
                ]);
            }

            if (martinProfile != null)
            {
                images.AddRange([
                    new PortfolioImage
                    {
                        AsadorProfileId = martinProfile.Id,
                        ImageUrl = "https://placehold.co/600x400/3a2a1a/f59e0b?text=Estaca+Criolla",
                        IsApproved = null
                    },
                    new PortfolioImage
                    {
                        AsadorProfileId = martinProfile.Id,
                        ImageUrl = "https://placehold.co/600x400/3a2a1a/f59e0b?text=Parrilla+de+Campo",
                        IsApproved = null
                    }
                ]);
            }

            context.PortfolioImages.AddRange(images);
            await context.SaveChangesAsync();
        }

        if (!context.Events.Any())
        {
            var juan = await userManager.FindByEmailAsync("juan@email.com");
            var maria = await userManager.FindByEmailAsync("maria@email.com");

            if (juan != null)
            {
                context.Events.AddRange(
                    new Event
                    {
                        ClientId = juan.Id,
                        Date = DateTime.UtcNow.AddDays(15),
                        Time = new TimeSpan(12, 0, 0),
                        City = "Buenos Aires",
                        Address = "Av. Corrientes 1234",
                        PeopleCount = 15,
                        EventType = "Cumpleaños",
                        ServiceDesired = "Parrilla completa",
                        Notes = "Cerca del mediodía, ideal si traen todo el equipo",
                        Status = "Disponible"
                    },
                    new Event
                    {
                        ClientId = juan.Id,
                        Date = DateTime.UtcNow.AddDays(30),
                        Time = new TimeSpan(20, 0, 0),
                        City = "Buenos Aires",
                        Address = "Calle Florida 567",
                        PeopleCount = 8,
                        EventType = "Cena familiar",
                        ServiceDesired = "Asado criollo",
                        Status = "Disponible"
                    }
                );
            }

            if (maria != null)
            {
                context.Events.Add(
                    new Event
                    {
                        ClientId = maria.Id,
                        Date = DateTime.UtcNow.AddDays(20),
                        Time = new TimeSpan(11, 0, 0),
                        City = "La Plata",
                        Address = "Calle 7 y 50",
                        PeopleCount = 25,
                        EventType = "Evento empresarial",
                        ServiceDesired = "Parrilla con ensaladas y acompañamientos",
                        Notes = "Preferentemente con experiencia en eventos corporativos",
                        Status = "Disponible"
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
