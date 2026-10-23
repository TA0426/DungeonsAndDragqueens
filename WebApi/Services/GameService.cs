namespace DungeonsAndDragqueens.WebApi.Services;

using Microsoft.EntityFrameworkCore;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.Core.Services;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using System.Text.Json;


public class GameService(
    StoreMonstersContext database,
    FightService fightService,
    CharacterService characterService)
{
    public async Task<CombatResult> StartFightAsync(
        Guid characterId,
        Guid monsterId,
        string userId)
    {

        var character = await database.Characters
            .FirstOrDefaultAsync(character => character.Id == characterId && character.UserId == userId);


        if (character is null)
            throw new KeyNotFoundException("Character not found.");


        var monster = await database.StoreMonsters.FindAsync(monsterId);
        if (monster is null)
            throw new KeyNotFoundException("Monster not found.");

        var progress =
            await database.CharacterMonsterProgress.FirstOrDefaultAsync(
                progress =>
                    progress.CharacterId == characterId &&
                    progress.MonsterId == monsterId);
        if (progress is null)
        {
            progress = new CharacterMonsterProgress
            {
                CharacterId = characterId,
                MonsterId = monsterId,
                RemainingQuantity = monster.Quantity
            };

            database.CharacterMonsterProgress.Add(progress);
        }

        if (progress.RemainingQuantity <= 0)
            throw new InvalidOperationException("This character has defeated all monsters of this type.");

        var enemiesBeforeFight = progress.RemainingQuantity;
        StoreSword? sword = null;
        if (character.SwordId is not null)
        {
            sword = await database.Swords.FindAsync(character.SwordId.Value);
        }

        var armorIds = new[] { character.BootsId, character.ChestId, character.LegsId, character.GlovesId, character.HelmetId }
            .Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var armor = await database.Armors.Where(piece => armorIds.Contains(piece.Id)).ToListAsync();
        var armorLevels = armor.ToDictionary(piece => piece.Id, piece => piece.Id == character.BootsId ? character.BootsLevel : piece.Id == character.ChestId ? character.ChestLevel : piece.Id == character.LegsId ? character.LegsLevel : piece.Id == character.GlovesId ? character.GlovesLevel : piece.Id == character.HelmetId ? character.HelmetLevel : 1);
        var result = fightService.Fight(character, monster, sword, progress, armor, character.SwordLevel, armorLevels);
        result.EnemiesBeforeFight = enemiesBeforeFight;
        result.EnemiesAfterFight = progress.RemainingQuantity;
        character.Round++;
        await database.SaveChangesAsync();

        if (!result.CharacterWon)
        {
            if (character.IsHardcore)
            {
                database.Characters.Remove(character);
                result.Message =
                    $"{character.Name} died and was removed from the game (Hardcore).";
            }
            else
            {
                result.Message =
                    $"{character.Name} was defeated but survived (Casual). Go to camp to recover.";
            }

            await database.SaveChangesAsync();

            return result;
        }

        await database.SaveChangesAsync();

        if (result.CharacterWon)
        {
            var account = await database.PlayerAccounts
                .FirstAsync(account => account.Id == Guid.Parse(userId));
            result.GoldEarned = Math.Max(5, monster.XPReward / 2);
            account.Gold += result.GoldEarned;
            await database.SaveChangesAsync();

            var tier = monster.XPReward >= 250 ? 3 : monster.XPReward >= 100 ? 2 : 1;
            var dropChance = tier switch { 1 => 15, 2 => 25, _ => 45 };
            if (Random.Shared.Next(100) >= dropChance)
            {
                result.Message = "You defeated the monster, but found no loot.";
            }
            else
            {
                var swords = await database.Swords.AsNoTracking().ToListAsync();
                var armors = await database.Armors.AsNoTracking().ToListAsync();
                var pool = swords.Where(item => (item.Damage <= 20 ? 1 : item.Damage <= 32 ? 2 : 3) == tier)
                    .Select(item => new LootChoice("sword", item.Id, item.Name, "", 1, item.Damage, 0, 0, item.AsciiArt))
                    .Concat(armors.Where(item => (item.Defense <= 3 ? 1 : item.Defense <= 7 ? 2 : 3) == tier)
                        .Select(item => new LootChoice("armor", item.Id, item.Name, item.Slot, 1, 0, item.Defense, item.SwordDamageBonus, item.AsciiArt)))
                    .OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
                if (pool.Count == 3)
                {
                    var offer = new CharacterLootOffer { CharacterId = characterId, ChoicesJson = JsonSerializer.Serialize(pool) };
                    database.CharacterLootOffers.Add(offer);
                    result.LootOfferId = offer.Id;
                    result.LootChoices = pool;
                    result.Message = "Choose one of the three loot rewards.";
                    await database.SaveChangesAsync();
                }
            }
        }

        var monsterCatalog = await database.StoreMonsters
            .AsNoTracking()
            .Select(monster => new { monster.Id, monster.Quantity })
            .ToListAsync();
        var remainingByMonster = await database.CharacterMonsterProgress
            .Where(progress => progress.CharacterId == characterId)
            .ToDictionaryAsync(progress => progress.MonsterId, progress => progress.RemainingQuantity);
        var allMonstersDefeated = monsterCatalog.All(monster =>
            remainingByMonster.GetValueOrDefault(monster.Id, monster.Quantity) <= 0);

        result.AllMonstersDefeated = allMonstersDefeated;

        if (allMonstersDefeated)
        {
            result.Message =
                "Congratulations! All monsters have been defeated!";
        }

        result.BattleLog.Add($"You earned {result.GoldEarned} gold.");
        result.Message += $" (+{result.GoldEarned} gold)";
        return result;
    }

    public async Task<List<DungeonResponse>> GetDungeonsAsync(Guid? characterId, string userId)
    {
        if (characterId.HasValue && !await database.Characters.AnyAsync(item => item.Id == characterId && item.UserId == userId))
            throw new KeyNotFoundException("Character not found.");
        var progress = characterId.HasValue
            ? await database.CharacterDungeonProgress.Where(item => item.CharacterId == characterId).ToDictionaryAsync(item => item.DungeonId)
            : new Dictionary<string, CharacterDungeonProgress>();
        var clearCounts = progress.ToDictionary(item => item.Key, item => item.Value.ClearCount);
        var dungeons = DungeonCatalog.All.Select((dungeon, index) =>
        {
            var state = progress.GetValueOrDefault(dungeon.Id);
            var wave = state?.CurrentWave ?? 1;
            var clearCount = state?.ClearCount ?? 0;
            return new DungeonResponse(dungeon.Id, dungeon.Name, dungeon.World, dungeon.Difficulty,
                dungeon.RecommendedLevel, dungeon.RecommendedSwordDamage, dungeon.Description,
                dungeon.AsciiArt, wave, clearCount > 0, DungeonProgression.IsBossWave(dungeon.Id, wave),
                DungeonProgression.IsUnlocked(index, clearCounts), clearCount, false);
        }).ToList();
        if (characterId.HasValue && DungeonProgression.IsEndlessUnlocked(clearCounts))
        {
            var state = progress.GetValueOrDefault(DungeonCatalog.EndlessId);
            var wave = state?.CurrentWave ?? 1;
            dungeons.Add(new DungeonResponse(
                DungeonCatalog.Endless.Id, DungeonCatalog.Endless.Name, DungeonCatalog.Endless.World,
                DungeonCatalog.Endless.Difficulty, DungeonCatalog.Endless.RecommendedLevel,
                DungeonCatalog.Endless.RecommendedSwordDamage, DungeonCatalog.Endless.Description,
                DungeonCatalog.Endless.AsciiArt, wave, false,
                DungeonProgression.IsBossWave(DungeonCatalog.EndlessId, wave), true,
                state?.ClearCount ?? 0, true));
        }
        return dungeons;
    }

    public async Task<CombatResult> StartDungeonFightAsync(Guid characterId, string dungeonId, string userId)
    {
        var character = await database.Characters.FirstOrDefaultAsync(item => item.Id == characterId && item.UserId == userId)
            ?? throw new KeyNotFoundException("Character not found.");
        var dungeon = DungeonCatalog.Find(dungeonId) ?? throw new KeyNotFoundException("Dungeon not found.");
        var endless = dungeonId == DungeonCatalog.EndlessId;
        var clearCounts = await database.CharacterDungeonProgress
            .Where(item => item.CharacterId == characterId)
            .ToDictionaryAsync(item => item.DungeonId, item => item.ClearCount);
        if (endless && !DungeonProgression.IsEndlessUnlocked(clearCounts))
            throw new InvalidOperationException("Clear all five dungeons to unlock Endless Descent.");
        var dungeonIndex = DungeonCatalog.IndexOf(dungeonId);
        if (!endless && !DungeonProgression.IsUnlocked(dungeonIndex, clearCounts))
            throw new InvalidOperationException($"Clear {DungeonCatalog.All[dungeonIndex - 1].Name} first to unlock {dungeon.Name}.");
        var progress = await database.CharacterDungeonProgress.FirstOrDefaultAsync(item => item.CharacterId == characterId && item.DungeonId == dungeonId);
        if (progress is null)
        {
            progress = new CharacterDungeonProgress { CharacterId = characterId, DungeonId = dungeonId };
            database.CharacterDungeonProgress.Add(progress);
        }

        var wave = progress.CurrentWave;
        var isBoss = DungeonProgression.IsBossWave(dungeonId, wave);
        string monsterName;
        if (isBoss)
            monsterName = endless
                ? DungeonCatalog.All[((wave / 10) - 1) % DungeonCatalog.All.Count].BossName
                : dungeon.BossName;
        else
        {
            var pool = endless
                ? dungeon.EnemyWaves[0]
                : dungeon.EnemyWaves[wave <= 16 ? 0 : wave <= 34 ? 1 : 2];
            monsterName = pool[Random.Shared.Next(pool.Length)];
        }
        var template = await database.StoreMonsters.FirstOrDefaultAsync(item => item.Name == monsterName)
            ?? throw new InvalidOperationException($"Monster '{monsterName}' is missing from the monster catalog.");
        var scale = DungeonProgression.GetMonsterScale(dungeonId, wave, progress.ClearCount);
        var monster = new StoreMonster
        {
            Id = template.Id,
            Name = template.Name,
            TypeOfMonster = template.TypeOfMonster,
            Quantity = 1,
            Hp = DungeonProgression.ScaleStat(template.Hp, scale.Hp),
            Damage = DungeonProgression.ScaleStat(template.Damage, scale.Damage),
            XPReward = template.XPReward,
            Description = template.Description
        };

        StoreSword? sword = character.SwordId is Guid swordId ? await database.Swords.FindAsync(swordId) : null;
        var armorIds = new[] { character.BootsId, character.ChestId, character.LegsId, character.GlovesId, character.HelmetId }
            .Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var armor = await database.Armors.Where(item => armorIds.Contains(item.Id)).ToListAsync();
        var armorLevels = armor.ToDictionary(item => item.Id, item => item.Id == character.BootsId ? character.BootsLevel : item.Id == character.ChestId ? character.ChestLevel : item.Id == character.LegsId ? character.LegsLevel : item.Id == character.GlovesId ? character.GlovesLevel : character.HelmetLevel);
        var result = fightService.Fight(character, monster, sword, new CharacterMonsterProgress { RemainingQuantity = 1 }, armor, character.SwordLevel, armorLevels);
        result.DungeonId = dungeon.Id;
        result.DungeonName = dungeon.Name;
        result.DungeonWave = wave;
        result.IsBoss = isBoss;
        character.Round++;
        await database.SaveChangesAsync();

        if (!result.CharacterWon)
        {
            if (character.IsHardcore)
            {
                database.Characters.Remove(character);
                result.Message = $"{character.Name} died in {dungeon.Name} (Hardcore).";
            }
            else result.Message = $"{character.Name} was defeated at {dungeon.Name}, wave {wave}. Recover at camp and try again.";
            await database.SaveChangesAsync();
            return result;
        }

        if (isBoss && !endless)
        {
            progress.IsCompleted = true;
            progress.ClearCount++;
            progress.CurrentWave = 1;
            result.DungeonCompleted = true;
        }
        else progress.CurrentWave++;

        var account = await database.PlayerAccounts.FirstAsync(item => item.Id == Guid.Parse(userId));
        result.GoldEarned = Math.Max(5, monster.XPReward / 2);
        account.Gold += result.GoldEarned;
        result.Message = isBoss
            ? endless
                ? $"ENDLESS BOSS DEFEATED! You survived wave {wave}. Next: wave {progress.CurrentWave}."
                : $"BOSS DEFEATED! You cleared {dungeon.Name}."
            : $"Wave {wave} cleared in {dungeon.Name}. Next: wave {progress.CurrentWave}.";

        var lootChance = DungeonProgression.GetLootChance(dungeonId, wave);
        if (Random.Shared.Next(100) < lootChance)
        {
            var tier = DungeonProgression.GetLootTier(dungeonId, wave);
            var offer = await CreateLootOfferAsync(characterId, tier);
            if (offer is not null)
            {
                result.LootOfferId = offer.Id;
                result.LootChoices = JsonSerializer.Deserialize<List<LootChoice>>(offer.ChoicesJson) ?? [];
                result.Message += " Choose one of three loot rewards.";
            }
        }
        else result.Message += " No loot this time.";
        result.BattleLog.Add($"You earned {result.GoldEarned} gold.");
        await database.SaveChangesAsync();
        return result;
    }

    private async Task<CharacterLootOffer?> CreateLootOfferAsync(Guid characterId, int tier)
    {
        var swords = await database.Swords.AsNoTracking().ToListAsync();
        var armors = await database.Armors.AsNoTracking().ToListAsync();
        var options = swords.Where(item => (item.Damage <= 20 ? 1 : item.Damage <= 32 ? 2 : 3) == tier)
            .Select(item => new LootChoice("sword", item.Id, item.Name, "", 1, item.Damage, 0, 0, item.AsciiArt))
            .Concat(armors.Where(item => (item.Defense <= 3 ? 1 : item.Defense <= 7 ? 2 : 3) == tier)
                .Select(item => new LootChoice("armor", item.Id, item.Name, item.Slot, 1, 0, item.Defense, item.SwordDamageBonus, item.AsciiArt)))
            .OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
        if (options.Count < 3) return null;
        var offer = new CharacterLootOffer { CharacterId = characterId, ChoicesJson = JsonSerializer.Serialize(options) };
        database.CharacterLootOffers.Add(offer);
        return offer;
    }

    public async Task<LootChoice> ClaimLootAsync(Guid offerId, int choiceIndex, string userId)
    {
        await using var transaction = await database.Database.BeginTransactionAsync();
        var offer = await database.CharacterLootOffers.FirstOrDefaultAsync(item => item.Id == offerId && database.Characters.Any(character => character.Id == item.CharacterId && character.UserId == userId));
        if (offer is null || offer.IsClaimed) throw new InvalidOperationException("Loot offer is missing or already claimed.");
        var choices = JsonSerializer.Deserialize<List<LootChoice>>(offer.ChoicesJson) ?? [];
        if (choiceIndex < 0 || choiceIndex >= choices.Count) throw new InvalidOperationException("Choose one of the offered items.");
        var choice = choices[choiceIndex];
        offer.IsClaimed = true;
        await database.SaveChangesAsync();
        // Inventory mutations are persisted together with the offer claim.
        await characterService.AddLootToInventoryAsync(offer.CharacterId, choice);
        await database.SaveChangesAsync();
        await transaction.CommitAsync();
        return choice;
    }
}
