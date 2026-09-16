using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.Core.Services;

namespace Calculator.Tests;


public class UnitTest1
{
    [Fact]
    public void CalculateLevel_ReturnsCorrectLevel()
    {
        var calculator = new LevelCalculator();

        var level = calculator.CalculateLevel(100);

        Assert.Equal(2, level);
    }

    [Fact]
    public void Fight_AddsSwordDamage()
    {
        var character = new StoreCharacter
        {
            Name = "Anna",
            Hp = 300,
            Damage = 10,
            Level = 1
        };

        var monster = new StoreMonster
        {
            Name = "Ghoul",
            Hp = 40,
            Damage = 5,
            XPReward = 20
        };

        var sword = new StoreSword
        {
            Name = "Steel Sword",
            Damage = 30
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 2
        };

        var fightService = new FightService(
            new LevelCalculator());

        var result = fightService.Fight(
            character,
            monster,
            sword,
            progress);

        Assert.True(result.CharacterWon);
        Assert.Equal(0, result.MonsterHpAfterFight);
        Assert.Equal(1, progress.RemainingQuantity);
    }

    [Fact]
    public void Fight_AddsXp_WhenMonsterDies()
    {
        var character = new StoreCharacter
        {
            Name = "Anna",
            Hp = 300,
            Damage = 100,
            Level = 1
        };

        var monster = new StoreMonster
        {
            Name = "Weak Monster",
            Hp = 20,
            Damage = 5,
            XPReward = 50
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 1
        };

        var fightService = new FightService(
            new LevelCalculator());

        fightService.Fight(
            character,
            monster,
            null,
            progress);

        Assert.Equal(50, character.XP);
    }

    [Fact]
    public void Fight_CharacterDies_WhenMonsterWins()
    {
        var character = new StoreCharacter
        {
            Name = "Anna",
            Hp = 10,
            Damage = 1,
            Level = 1
        };

        var monster = new StoreMonster
        {
            Name = "Strong Monster",
            Hp = 100,
            Damage = 20,
            XPReward = 50
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 1
        };

        var fightService = new FightService(
            new LevelCalculator());

        var result = fightService.Fight(
            character,
            monster,
            null,
            progress);

        Assert.False(result.CharacterWon);
        Assert.Equal(0, result.CharacterHpAfterFight);
        Assert.Equal(1, progress.RemainingQuantity);
    }
}