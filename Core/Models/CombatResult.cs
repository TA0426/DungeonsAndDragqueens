namespace DungeonsAndDragqueens.Core;

public class CombatResult
{
    public int EnemiesBeforeFight { get; set; }
    public int EnemiesAfterFight { get; set; }
    public StoreSword? Loot { get; set; }
    public Guid? LootOfferId { get; set; }
    public List<LootChoice> LootChoices { get; set; } = [];
    public string MonsterArt { get; set; } = "";
    public string DungeonId { get; set; } = "";
    public string DungeonName { get; set; } = "";
    public int DungeonWave { get; set; }
    public bool IsBoss { get; set; }
    public bool DungeonCompleted { get; set; }
    public bool AllMonstersDefeated { get; set; }
    public string Message { get; set; } = "";
    public string CharacterName { get; set; } = "";
    public string MonsterName { get; set; } = "";
    public int CharacterHpAfterFight { get; set; }
    public int CharacterMaxHp { get; set; }
    public int MonsterHpAfterFight { get; set; }
    public int MonsterMaxHp { get; set; }
    public bool CharacterWon { get; set; }
    public int XpGained { get; set; }
    public int GoldEarned { get; set; }
    public int NewLevel { get; set; }
    public List<string> BattleLog { get; set; } = [];
}
