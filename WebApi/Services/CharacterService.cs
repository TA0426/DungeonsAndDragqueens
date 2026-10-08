namespace DungeonsAndDragqueens.WebApi.Services;

using Microsoft.EntityFrameworkCore;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.Core.Services;
using DungeonsAndDragqueens.WebApi.DatabaseContext;

public class CharacterService(StoreMonstersContext database)
{
    public Task<List<StoreCharacter>> GetAsync(string userId)
    {
        return database.Characters
            .Where(character => character.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }
    public Task<StoreCharacter?> GetAsync(Guid id, string userId)
    {
        return database.Characters
            .AsNoTracking()
            .FirstOrDefaultAsync(character =>
                character.Id == id &&
                character.UserId == userId);
    }
    public async Task<StoreCharacter> CreateAsync(
        CreateCharacterDTO dto,
        string userId)
    {
        var character = new StoreCharacter
        {
            Name = dto.Name,
            UserId = userId
        };

        database.Characters.Add(character);
        await database.SaveChangesAsync();

        return character;
    }
    public async Task<StoreCharacter?> EquipSwordAsync(
        Guid characterId,
        Guid swordId,
        string userId)
    {
        var character = await database.Characters
            .FirstOrDefaultAsync(character =>
                character.Id == characterId &&
                character.UserId == userId);
        var sword = await database.Swords.FindAsync(swordId);

        if (character is null || sword is null)
            return null;

        character.SwordId = sword.Id;

        await database.SaveChangesAsync();

        return character;
    }
    public async Task<CampResult> CampAsync(
        Guid characterId,
        string userId)
    {
        var character = await database.Characters
            .FirstOrDefaultAsync(character =>
                character.Id == characterId &&
                character.UserId == userId);

        if (character is null)
            return new CampResult(null, 0);

        var roundsSinceCamp =
            character.Round - character.LastCampRound;

        if (roundsSinceCamp < 3)
        {
            var roundsLeft = 3 - roundsSinceCamp;
            return new CampResult(character, roundsLeft);
        }

        character.Hp = character.MaxHp;
        character.LastCampRound = character.Round;

        await database.SaveChangesAsync();

        return new CampResult(character, 0);
    }
}

public record CampResult(
    StoreCharacter? Character,
    int RoundsLeft);

