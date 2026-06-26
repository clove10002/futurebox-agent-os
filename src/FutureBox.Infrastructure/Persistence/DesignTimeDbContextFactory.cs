using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FutureBox.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FutureBoxDbContext>
{
    public FutureBoxDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FutureBoxDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=FutureBoxDb;Trusted_Connection=true;",
            sql => sql.MigrationsAssembly(typeof(FutureBoxDbContext).Assembly.FullName));

        return new FutureBoxDbContext(optionsBuilder.Options);
    }
}
