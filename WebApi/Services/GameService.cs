namespace OppgaveUkeEnModul3.WebApi.Services;

using Microsoft.EntityFrameworkCore;
using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.Core.Services;
using OppgaveUkeEnModul3.WebApi.DatabaseContext;

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
        var enemiesBeforeFight = monster.Quantity;

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
        StoreSword? sword = null;
        if (character.SwordId is not null)
        {
            sword = await database.Swords.FindAsync(character.SwordId.Value);
        }

        var result = fightService.Fight(character, monster, sword, progress);
        result.EnemiesBeforeFight = enemiesBeforeFight;
        result.EnemiesAfterFight = monster.Quantity;
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

        var allMonstersDefeated =
            !await database.StoreMonsters
            .AnyAsync(monster => monster.Quantity > 0);

        result.AllMonstersDefeated = allMonstersDefeated;

        if (allMonstersDefeated)
        {
            result.Message =
                "Congratulations! All monsters have been defeated!";
        }
        return result;
    }
}