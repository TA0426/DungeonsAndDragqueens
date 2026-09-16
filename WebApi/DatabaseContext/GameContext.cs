namespace OppgaveUkeEnModul3.WebApi.DatabaseContext;

using Microsoft.EntityFrameworkCore;
using OppgaveUkeEnModul3.Core;

public class GameContext(
    DbContextOptions<GameContext> options)
    : DbContext(options)
{
    public DbSet<StoreCharacter> Characters => Set<StoreCharacter>();

    public DbSet<StoreMonster> Monsters => Set<StoreMonster>();

    public DbSet<StoreSword> Swords => Set<StoreSword>();
}