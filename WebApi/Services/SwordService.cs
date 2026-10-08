using Microsoft.EntityFrameworkCore;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.DatabaseContext;

namespace DungeonsAndDragqueens.WebApi.Services;

public class SwordService(StoreMonstersContext database)
{
    public Task<List<StoreSword>> GetAsync()
    {
        return database.Swords
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<StoreSword?> GetAsync(Guid id)
    {
        return database.Swords
            .AsNoTracking()
            .FirstOrDefaultAsync(sword => sword.Id == id);
    }

    public async Task<StoreSword> CreateAsync(CreateSwordDTO dto)
    {
        var sword = new StoreSword
        {
            Name = dto.Name,
            Damage = dto.Damage,
            Description = dto.Description
        };

        database.Swords.Add(sword);
        await database.SaveChangesAsync();

        return sword;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sword = await database.Swords.FindAsync(id);

        if (sword is null)
            return false;

        database.Swords.Remove(sword);
        await database.SaveChangesAsync();

        return true;
    }
}