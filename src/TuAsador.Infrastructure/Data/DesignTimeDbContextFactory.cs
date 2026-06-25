using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TuAsador.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TuAsadorDbContext>
{
    public TuAsadorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TuAsadorDbContext>();

        var connectionString = args.Length > 0
            ? args[0]
            : "Server=NOTEBOOKEZE\\LOCALSERVER;Database=TuAsadorDev;Trusted_Connection=True;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new TuAsadorDbContext(optionsBuilder.Options);
    }
}
