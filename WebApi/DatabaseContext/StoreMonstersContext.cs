namespace OppgaveUkeEnModul3.WebApi.DatabaseContext;

using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.Core.Interfaces;

using Microsoft.EntityFrameworkCore;

public class StoreMonstersContext(
    DbContextOptions<StoreMonstersContext> options)
    : DbContext(options), IStoreMonstersRepository
{
    public DbSet<StoreMonster> StoreMonsters => Set<StoreMonster>();
    public DbSet<StoreCharacter> Characters => Set<StoreCharacter>();
    public DbSet<StoreSword> Swords => Set<StoreSword>();
    public DbSet<CharacterMonsterProgress> CharacterMonsterProgress =>
    Set<CharacterMonsterProgress>();

    public StoreMonster Add(StoreMonster monster)
    {
        StoreMonsters.Add(monster);
        SaveChanges();
        return monster;
    }


    public async Task<StoreMonster> AddAsync(StoreMonster monster)
    {
        StoreMonsters.Add(monster);
        await SaveChangesAsync();
        return monster;
    }

    public IEnumerable<StoreMonster> Get() =>
        StoreMonsters.AsNoTracking();

    public StoreMonster? Get(Guid id) =>
        StoreMonsters.AsNoTracking()
            .FirstOrDefault(monster => monster.Id == id);

    public async Task<List<StoreMonster>> GetAsync(
        bool? outOfStock,
        int page,
        int pageSize,
        string? sortBy)
    {
        IQueryable<StoreMonster> query = StoreMonsters.AsNoTracking();

        if (outOfStock is true)
            query = query.Where(monster => monster.Quantity == 0);

        if (outOfStock is false)
            query = query.Where(monster => monster.Quantity > 0);

        query = sortBy?.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(monster => monster.Name),
            "quantity" => query.OrderBy(monster => monster.Quantity),
            "hp" => query.OrderBy(monster => monster.Hp),
            "damage" => query.OrderBy(monster => monster.Damage),
            "xp" => query.OrderBy(monster => monster.XPReward),
            null or "" => query,
            _ => throw new ArgumentException("Invalid sort option.")
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public Task<StoreMonster?> GetAsync(Guid id) =>
        StoreMonsters.FirstOrDefaultAsync(monster => monster.Id == id);

    public bool Remove(Guid id)
    {
        var monster = StoreMonsters.Find(id);

        if (monster is null)
            return false;

        StoreMonsters.Remove(monster);
        SaveChanges();
        return true;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var monster = await StoreMonsters.FindAsync(id);

        if (monster is null)
            return false;

        StoreMonsters.Remove(monster);
        await SaveChangesAsync();
        return true;
    }
}
