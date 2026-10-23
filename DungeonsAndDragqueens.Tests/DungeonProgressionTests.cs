using DungeonsAndDragqueens.WebApi.Services;

namespace DungeonsAndDragqueens.Tests;

public class DungeonProgressionTests
{
    [Fact]
    public void NextDungeonUnlocksAfterPreviousBossIsCleared()
    {
        var clears = new Dictionary<string, int>();

        Assert.True(DungeonProgression.IsUnlocked(0, clears));
        Assert.False(DungeonProgression.IsUnlocked(1, clears));

        clears[DungeonCatalog.All[0].Id] = 1;

        Assert.True(DungeonProgression.IsUnlocked(1, clears));
    }

    [Fact]
    public void EndlessUnlocksOnlyAfterEveryCampaignDungeonIsCleared()
    {
        var clears = DungeonCatalog.All.ToDictionary(dungeon => dungeon.Id, _ => 1);
        clears[DungeonCatalog.All[^1].Id] = 0;

        Assert.False(DungeonProgression.IsEndlessUnlocked(clears));

        clears[DungeonCatalog.All[^1].Id] = 1;
        Assert.True(DungeonProgression.IsEndlessUnlocked(clears));
    }

    [Fact]
    public void LaterDungeonsAndReplayWavesScaleUpMonsterStats()
    {
        var firstDungeon = DungeonProgression.GetMonsterScale(DungeonCatalog.All[0].Id, 10, 0);
        var nextDungeon = DungeonProgression.GetMonsterScale(DungeonCatalog.All[1].Id, 10, 0);
        var replay = DungeonProgression.GetMonsterScale(DungeonCatalog.All[0].Id, 10, 1);
        var endlessLaterWave = DungeonProgression.GetMonsterScale(DungeonCatalog.EndlessId, 50, 0);

        Assert.True(nextDungeon.Hp > firstDungeon.Hp);
        Assert.True(nextDungeon.Damage > firstDungeon.Damage);
        Assert.True(replay.Hp > firstDungeon.Hp);
        Assert.True(endlessLaterWave.Hp > nextDungeon.Hp);
    }

    [Theory]
    [InlineData("echo-caverns", 1, 1)]
    [InlineData("echo-caverns", 20, 2)]
    [InlineData("echo-caverns", 40, 3)]
    [InlineData(DungeonCatalog.EndlessId, 10, 2)]
    [InlineData(DungeonCatalog.EndlessId, 40, 3)]
    public void LootTierRequiresHarderWaves(string dungeonId, int wave, int expectedTier) =>
        Assert.Equal(expectedTier, DungeonProgression.GetLootTier(dungeonId, wave));

    [Fact]
    public void EndlessBossesArriveEveryTenWaves()
    {
        Assert.False(DungeonProgression.IsBossWave(DungeonCatalog.EndlessId, 9));
        Assert.True(DungeonProgression.IsBossWave(DungeonCatalog.EndlessId, 10));
        Assert.True(DungeonProgression.IsBossWave(DungeonCatalog.EndlessId, 20));
    }
}
