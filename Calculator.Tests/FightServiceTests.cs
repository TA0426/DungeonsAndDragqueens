using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.Core.Services;

namespace Calculator.Tests;

public class FightServiceTests
{

    [Fact]
    public void NewCharacter_StartsAtLevelOne()
    {
        var calculator = new LevelCalculator();

        var level = calculator.CalculateLevel(0);

        Assert.Equal(1, level);
    }

    [Fact]
    public void WinningFight_ReducesMonsterQuantity()
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
            Name = "Ghoul",
            Hp = 20,
            Damage = 5,
            XPReward = 40,
            Quantity = 3
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 3
        };

        var service = new FightService(
            new LevelCalculator());

        service.Fight(
            character,
            monster,
            null,
            progress);

        Assert.Equal(2, progress.RemainingQuantity);
    }

    [Fact]
    public void Fight_CharacterWins_WhenMonsterDies()
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
            Name = "Ghoul",
            Hp = 20,
            Damage = 5,
            XPReward = 40,
            Quantity = 2
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 2
        };

        var service = new FightService(
            new LevelCalculator());

        var result = service.Fight(
            character,
            monster,
            null,
            progress);

        Assert.True(result.CharacterWon);
    }

    [Fact]
    public void Fight_UsesSwordDamage()
    {
        var character = new StoreCharacter
        {
            Name = "Anna",
            Hp = 300,
            Damage = 10
        };

        var monster = new StoreMonster
        {
            Name = "Ghoul",
            Hp = 30,
            Damage = 1,
            XPReward = 0
        };

        var sword = new StoreSword
        {
            Name = "Steel Sword",
            Damage = 20
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 1
        };

        var service = new FightService(
            new LevelCalculator());

        var result = service.Fight(
            character,
            monster,
            sword,
            progress);

        Assert.Equal(0, result.MonsterHpAfterFight);
        Assert.Equal(30, character.Damage);
    }

    [Fact]
    public void WinningFight_GivesCharacterXp()
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
            Name = "Ghoul",
            Hp = 20,
            Damage = 1,
            XPReward = 100
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 1
        };

        var service = new FightService(
            new LevelCalculator());

        service.Fight(
            character,
            monster,
            null,
            progress);

        Assert.Equal(100, character.XP);
        Assert.Equal(2, character.Level);
    }

    [Fact]
    public void Fight_CharacterDies_WhenMonsterWins()
    {
        var character = new StoreCharacter
        {
            Hp = 10,
            Damage = 1
        };

        var monster = new StoreMonster
        {
            Hp = 100,
            Damage = 20
        };

        var progress = new CharacterMonsterProgress
        {
            RemainingQuantity = 1
        };

        var service = new FightService(
            new LevelCalculator());

        var result = service.Fight(
            character,
            monster,
            null,
            progress);

        Assert.False(result.CharacterWon);
        Assert.Equal(0, result.CharacterHpAfterFight);
        Assert.Equal(1, progress.RemainingQuantity);
    }
}