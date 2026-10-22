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
            Name = $"{dto.Name.Trim()} the {dto.CharacterClass}",
            CharacterClass = dto.CharacterClass,
            IsHardcore = dto.IsHardcore,
            UserId = userId
        };

        var starterSword = await database.Swords.FirstOrDefaultAsync(item => item.Name == "Tin Dagger");
        if (starterSword is not null)
        {
            character.SwordId = starterSword.Id;
            database.CharacterInventoryItems.Add(new CharacterInventoryItem
            {
                CharacterId = character.Id,
                SwordId = starterSword.Id,
                Level = 1,
                Quantity = 1
            });
        }

        database.Characters.Add(character);
        await database.SaveChangesAsync();

        return character;
    }
    public async Task<StoreCharacter?> EquipSwordAsync(
        Guid characterId,
        Guid swordId,
        int level,
        string userId)
    {
        var character = await database.Characters
            .FirstOrDefaultAsync(character =>
                character.Id == characterId &&
                character.UserId == userId);
        var owned = await database.CharacterInventoryItems.AnyAsync(item =>
            item.CharacterId == characterId && item.SwordId == swordId && item.Level == level && item.Quantity > 0);
        var sword = await database.Swords.FindAsync(swordId);

        if (character is null || sword is null || !owned || level is < 1 or > 3)
            return null;

        character.SwordId = sword.Id;
        character.SwordLevel = level;

        await database.SaveChangesAsync();

        return character;
    }

    public Task<StoreSword?> GetSwordAsync(Guid swordId)
    {
        return database.Swords
            .AsNoTracking()
            .FirstOrDefaultAsync(sword => sword.Id == swordId);
    }

    public async Task<StoreCharacter?> EquipArmorAsync(
        Guid characterId, Guid armorId, int level, string slot, string userId)
    {
        var character = await database.Characters.FirstOrDefaultAsync(item =>
            item.Id == characterId && item.UserId == userId);
        var armor = await database.Armors.FindAsync(armorId);
        var owned = await database.CharacterInventoryItems.AnyAsync(item =>
            item.CharacterId == characterId && item.ArmorId == armorId && item.Level == level && item.Quantity > 0);
        if (character is null || armor is null || !owned || level is < 1 or > 3 ||
            !string.Equals(armor.Slot, slot, StringComparison.OrdinalIgnoreCase))
            return null;

        switch (slot.ToLowerInvariant())
        {
            case "boots": character.BootsId = armor.Id; character.BootsLevel = level; break;
            case "chest": character.ChestId = armor.Id; character.ChestLevel = level; break;
            case "legs": character.LegsId = armor.Id; character.LegsLevel = level; break;
            case "gloves": character.GlovesId = armor.Id; character.GlovesLevel = level; break;
            case "helmet": character.HelmetId = armor.Id; character.HelmetLevel = level; break;
            default: return null;
        }
        await database.SaveChangesAsync();
        return character;
    }

    public Task<List<StoreArmor>> GetArmorsAsync() => database.Armors.AsNoTracking().ToListAsync();
    public Task<StoreArmor?> GetArmorAsync(Guid id) => database.Armors.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);

    public async Task<List<InventoryItemResponse>> GetInventoryAsync(Guid characterId, string userId)
    {
        if (!await database.Characters.AnyAsync(item => item.Id == characterId && item.UserId == userId))
            throw new KeyNotFoundException("Character not found.");
        var inventory = await database.CharacterInventoryItems.Where(item => item.CharacterId == characterId && item.Quantity > 0).AsNoTracking().ToListAsync();
        var swordIds = inventory.Where(item => item.SwordId.HasValue).Select(item => item.SwordId!.Value).Distinct().ToList();
        var armorIds = inventory.Where(item => item.ArmorId.HasValue).Select(item => item.ArmorId!.Value).Distinct().ToList();
        var swords = await database.Swords.Where(item => swordIds.Contains(item.Id)).AsNoTracking().ToDictionaryAsync(item => item.Id);
        var armors = await database.Armors.Where(item => armorIds.Contains(item.Id)).AsNoTracking().ToDictionaryAsync(item => item.Id);
        return inventory.Select(item =>
        {
            var sword = item.SwordId is Guid swordId ? swords.GetValueOrDefault(swordId) : null;
            var armor = item.ArmorId is Guid armorId ? armors.GetValueOrDefault(armorId) : null;
            return new InventoryItemResponse(item.Id, item.SwordId, item.ArmorId, sword?.Name ?? armor?.Name ?? "Unknown gear", sword is null ? "armor" : "sword", armor?.Slot ?? "", item.Level, item.Quantity, GearStats.AtLevel(sword?.Damage ?? 0, item.Level), GearStats.AtLevel(armor?.Defense ?? 0, item.Level), GearStats.AtLevel(armor?.SwordDamageBonus ?? 0, item.Level), sword?.AsciiArt ?? armor?.AsciiArt ?? "");
        }).ToList();
    }

    public async Task AddLootToInventoryAsync(Guid characterId, LootChoice choice)
    {
        var matching = database.CharacterInventoryItems.Where(item => item.CharacterId == characterId && item.Level == choice.Level);
        var row = choice.Type == "sword"
            ? await matching.FirstOrDefaultAsync(item => item.SwordId == choice.ItemId)
            : await matching.FirstOrDefaultAsync(item => item.ArmorId == choice.ItemId);
        if (row is null)
        {
            row = new CharacterInventoryItem { CharacterId = characterId, Level = choice.Level, Quantity = 0 };
            if (choice.Type == "sword") row.SwordId = choice.ItemId;
            else row.ArmorId = choice.ItemId;
            database.CharacterInventoryItems.Add(row);
        }
        row.Quantity++;
        var emptyRows = new List<CharacterInventoryItem>();
        while (row.Quantity >= 3 && row.Level < 3)
        {
            row.Quantity -= 3;
            var upgradedLevel = row.Level + 1;
            var upgraded = await database.CharacterInventoryItems.FirstOrDefaultAsync(item => item.CharacterId == characterId && item.Level == upgradedLevel && (choice.Type == "sword" ? item.SwordId == choice.ItemId : item.ArmorId == choice.ItemId));
            if (upgraded is null)
            {
                upgraded = new CharacterInventoryItem { CharacterId = characterId, SwordId = choice.Type == "sword" ? choice.ItemId : null, ArmorId = choice.Type == "armor" ? choice.ItemId : null, Level = upgradedLevel, Quantity = 0 };
                database.CharacterInventoryItems.Add(upgraded);
            }
            upgraded.Quantity++;
            await UpgradeEquippedGearAsync(characterId, choice, upgradedLevel);
            if (row.Quantity == 0) emptyRows.Add(row);
            row = upgraded;
        }
        foreach (var emptyRow in emptyRows) database.CharacterInventoryItems.Remove(emptyRow);
        await AutoEquipIfSlotEmptyAsync(characterId, choice, row.Level);
    }

    private async Task AutoEquipIfSlotEmptyAsync(Guid characterId, LootChoice choice, int level)
    {
        var character = await database.Characters.FindAsync(characterId);
        if (character is null) return;

        if (choice.Type == "sword")
        {
            if (character.SwordId is null)
            {
                character.SwordId = choice.ItemId;
                character.SwordLevel = level;
            }
            return;
        }

        switch (choice.Slot.ToLowerInvariant())
        {
            case "boots" when character.BootsId is null:
                character.BootsId = choice.ItemId; character.BootsLevel = level; break;
            case "chest" when character.ChestId is null:
                character.ChestId = choice.ItemId; character.ChestLevel = level; break;
            case "legs" when character.LegsId is null:
                character.LegsId = choice.ItemId; character.LegsLevel = level; break;
            case "gloves" when character.GlovesId is null:
                character.GlovesId = choice.ItemId; character.GlovesLevel = level; break;
            case "helmet" when character.HelmetId is null:
                character.HelmetId = choice.ItemId; character.HelmetLevel = level; break;
        }
    }

    private async Task UpgradeEquippedGearAsync(Guid characterId, LootChoice choice, int newLevel)
    {
        var character = await database.Characters.FindAsync(characterId);
        if (character is null) return;
        if (choice.Type == "sword" && character.SwordId == choice.ItemId && character.SwordLevel == newLevel - 1) character.SwordLevel = newLevel;
        if (choice.Type != "armor") return;
        if (character.BootsId == choice.ItemId && character.BootsLevel == newLevel - 1) character.BootsLevel = newLevel;
        if (character.ChestId == choice.ItemId && character.ChestLevel == newLevel - 1) character.ChestLevel = newLevel;
        if (character.LegsId == choice.ItemId && character.LegsLevel == newLevel - 1) character.LegsLevel = newLevel;
        if (character.GlovesId == choice.ItemId && character.GlovesLevel == newLevel - 1) character.GlovesLevel = newLevel;
        if (character.HelmetId == choice.ItemId && character.HelmetLevel == newLevel - 1) character.HelmetLevel = newLevel;
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

        if (roundsSinceCamp < 3 && character.Hp > 0)
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

