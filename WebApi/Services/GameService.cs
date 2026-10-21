namespace DungeonsAndDragqueens.WebApi.Services;

using Microsoft.EntityFrameworkCore;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.Core.Services;
using DungeonsAndDragqueens.WebApi.DatabaseContext;


public class GameService(
    StoreMonstersContext database,
    FightService fightService)
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

        var result = fightService.Fight(character, monster, sword, progress);
        result.EnemiesBeforeFight = enemiesBeforeFight;
        result.EnemiesAfterFight = progress.RemainingQuantity;
        character.Round++;
        await database.SaveChangesAsync();

        if (!result.CharacterWon)
        {
            database.Characters.Remove(character);

            result.Message =
                $"{character.Name} died and was removed from the game.";

            await database.SaveChangesAsync();

            return result;
        }

        await database.SaveChangesAsync();

        if (result.CharacterWon)
        {
            var swords = await database.Swords
                .AsNoTracking()
                .ToListAsync();

            var noLoot = Random.Shared.Next(100) < 40;

            if (noLoot || swords.Count == 0)
            {
                result.Loot = null;
                result.Message = "You defeated the monster, but found no loot.";
            }
            else
            {
                var strongestSwordDamage =
                    swords.Max(sword => sword.Damage);

                var totalWeight = swords.Sum(sword =>
                    strongestSwordDamage - sword.Damage + 1);

                var randomNumber =
                    Random.Shared.Next(totalWeight);

                foreach (var lootSword in swords)
                {
                    var weight =
                        strongestSwordDamage - lootSword.Damage + 1;

                    if (randomNumber < weight)
                    {
                        result.Loot = lootSword;
                        break;
                    }

                    randomNumber -= weight;
                }

                result.Message =
                    $"You found {result.Loot?.Name}!";
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
        return result;
    }
}
