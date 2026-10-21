using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DungeonsAndDragqueens.WebApi.DatabaseContext;

public class StoreMonstersContextFactory
    : IDesignTimeDbContextFactory<StoreMonstersContext>
{
    public StoreMonstersContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=database;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<StoreMonstersContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new StoreMonstersContext(options);
    }
}
