namespace DungeonsAndDragqueens.Core.Services;

using DungeonsAndDragqueens.Core;

public class FightService
{
    private readonly LevelCalculator _levelCalculator;

    public FightService(LevelCalculator levelCalculator)
    {
        _levelCalculator = levelCalculator;
    }

    public CombatResult Fight(
    StoreCharacter character,
    StoreMonster monster,
    StoreSword? sword,
    CharacterMonsterProgress progress,
    IEnumerable<StoreArmor>? armor = null,
    int swordLevel = 1,
    IReadOnlyDictionary<Guid, int>? armorLevels = null)
    {
        var battleLog = new List<string>();
        var characterDamage = character.Damage;
        var monsterHp = monster.Hp;
        var round = 1;

        if (sword is not null)
        {
            characterDamage += GearStats.AtLevel(sword.Damage, swordLevel);

            battleLog.Add(
                $"{character.Name} uses {sword.Name}.");
        }

        var armorPieces = armor?.ToList() ?? [];
        var defense = armorPieces.Sum(piece => GearStats.AtLevel(piece.Defense, armorLevels?.GetValueOrDefault(piece.Id) ?? 1));
        var swordBonus = armorPieces.Sum(piece => GearStats.AtLevel(piece.SwordDamageBonus, armorLevels?.GetValueOrDefault(piece.Id) ?? 1));
        characterDamage += swordBonus;
        if (swordBonus > 0)
            battleLog.Add($"Armor grants +{swordBonus} sword damage.");

        while (character.Hp > 0 && monsterHp > 0)
        {
            battleLog.Add($"--- Round {round} ---");

            var monsterHpBefore = monsterHp;

            monsterHp = Math.Max(
                0,
                monsterHp - characterDamage);

            var damageDealt = monsterHpBefore - monsterHp;

            battleLog.Add(
                $"{character.Name} attacks {monster.Name} " +
                $"and deals {damageDealt} damage.");

            battleLog.Add(
                $"{monster.Name} has {monsterHp} HP left.");

            if (monsterHp == 0) // brukte <= før jeg endret til Math.Max
            {
                progress.RemainingQuantity--;

                var oldLevel = character.Level;

                character.XP += monster.XPReward;
                character.Level =
                    _levelCalculator.CalculateLevel(character.XP);

                var levelsGained = character.Level - oldLevel;

                if (levelsGained > 0)
                {
                    character.MaxHp += levelsGained * 50;
                    character.Damage += levelsGained * 10;

                    battleLog.Add(
                        $"LEVEL UP! {character.Name} is now level " +
                        $"{character.Level}.");

                    battleLog.Add(
                        $"Max HP increased to {character.MaxHp}.");

                    battleLog.Add(
                        $"Damage increased to {character.Damage}.");
                }

                battleLog.Add(
                    $"{monster.Name} is defeated.");

                battleLog.Add(
                    $"{character.Name} gains {monster.XPReward} XP.");

                if (character.Level > oldLevel)
                {
                    battleLog.Add(
                        $"LEVEL UP! {character.Name} is now level " +
                        $"{character.Level}.");
                }

                break;
            }

            var characterHpBefore = character.Hp;

            var damageAfterArmor = Math.Max(1, monster.Damage - defense);
            character.Hp = Math.Max(0, character.Hp - damageAfterArmor);

            var damageReceived =
                characterHpBefore - character.Hp;

            battleLog.Add(
                $"{monster.Name} attacks {character.Name} " +
                $"and deals {damageReceived} damage.");
            if (defense > 0)
                battleLog.Add($"Armor blocked {monster.Damage - damageReceived} damage.");

            battleLog.Add(
                $"{character.Name} has {character.Hp} HP left.");

            if (character.Hp <= 0)
            {
                battleLog.Add(
                    $"{character.Name} has been defeated.");
            }

            round++;
        }

        return new CombatResult
        {
            CharacterName = character.Name,
            MonsterName = monster.Name,
            CharacterHpAfterFight = character.Hp,
            CharacterMaxHp = character.MaxHp,
            MonsterHpAfterFight = monsterHp,
            MonsterMaxHp = monster.Hp,
            CharacterWon = monsterHp <= 0,
            XpGained = monsterHp <= 0
                ? monster.XPReward
                : 0,
            NewLevel = character.Level,
            BattleLog = battleLog
        };
    }
}
