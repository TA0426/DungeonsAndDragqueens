namespace OppgaveUkeEnModul3.Core.Services;

using OppgaveUkeEnModul3.Core;

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
    CharacterMonsterProgress progress)
    {
        var battleLog = new List<string>();
        var characterDamage = character.Damage;
        var monsterHp = monster.Hp;
        var round = 1;

        if (sword is not null)
        {
            characterDamage += sword.Damage;

            battleLog.Add(
                $"{character.Name} uses {sword.Name}.");
        }

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

            character.Hp = Math.Max(
                0,
                character.Hp - monster.Damage);

            var damageReceived =
                characterHpBefore - character.Hp;

            battleLog.Add(
                $"{monster.Name} attacks {character.Name} " +
                $"and deals {damageReceived} damage.");

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
            MonsterHpAfterFight = monsterHp,
            CharacterWon = monsterHp <= 0,
            XpGained = monsterHp <= 0
                ? monster.XPReward
                : 0,
            NewLevel = character.Level,
            BattleLog = battleLog
        };
    }
}