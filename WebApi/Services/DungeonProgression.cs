namespace DungeonsAndDragqueens.WebApi.Services;

public static class DungeonProgression
{
    public static bool IsUnlocked(int dungeonIndex, IReadOnlyDictionary<string, int> clearCounts) =>
        dungeonIndex == 0 || clearCounts.GetValueOrDefault(DungeonCatalog.All[dungeonIndex - 1].Id) > 0;

    public static bool IsEndlessUnlocked(IReadOnlyDictionary<string, int> clearCounts) =>
        DungeonCatalog.All.All(dungeon => clearCounts.GetValueOrDefault(dungeon.Id) > 0);

    public static bool IsBossWave(string dungeonId, int wave) =>
        dungeonId == DungeonCatalog.EndlessId ? wave % 10 == 0 : wave == 51;

    public static (double Hp, double Damage) GetMonsterScale(
        string dungeonId,
        int wave,
        int clearCount)
    {
        var endless = dungeonId == DungeonCatalog.EndlessId;
        var index = endless ? DungeonCatalog.All.Count : Math.Max(0, DungeonCatalog.IndexOf(dungeonId));
        var replayScale = endless ? 1 : 1 + clearCount * 0.1;
        var endlessStage = endless ? 1 + (wave - 1) / 50 * 0.15 : 1;
        var waveScale = 0.8 + wave * 0.024;
        var bossScale = IsBossWave(dungeonId, wave) ? 2.1 : 1;

        return (
            Math.Pow(1.45, index) * replayScale * endlessStage * waveScale * bossScale,
            Math.Pow(1.7, index) * replayScale * endlessStage * waveScale * bossScale);
    }

    public static int ScaleStat(int value, double scale) =>
        (int)Math.Clamp(Math.Ceiling(value * scale), 1, int.MaxValue);

    public static int GetLootTier(string dungeonId, int wave) =>
        wave >= 40 || (dungeonId != DungeonCatalog.EndlessId && IsBossWave(dungeonId, wave)) ? 3 :
        wave >= 20 || IsBossWave(dungeonId, wave) ? 2 : 1;

    public static int GetLootChance(string dungeonId, int wave) =>
        IsBossWave(dungeonId, wave) ? 100 : wave >= 40 ? 40 : wave >= 20 ? 25 : 15;
}
