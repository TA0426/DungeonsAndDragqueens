namespace DungeonsAndDragqueens.Core;

public record DungeonDefinition(
    string Id,
    string Name,
    string World,
    string Difficulty,
    int RecommendedLevel,
    int RecommendedSwordDamage,
    string Description,
    string[][] EnemyWaves,
    string BossName,
    string AsciiArt);

public record DungeonResponse(
    string Id,
    string Name,
    string World,
    string Difficulty,
    int RecommendedLevel,
    int RecommendedSwordDamage,
    string Description,
    string AsciiArt,
    int CurrentWave,
    bool IsCompleted,
    bool IsBossWave);
