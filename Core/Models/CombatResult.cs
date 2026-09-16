namespace OppgaveUkeEnModul3.Core;

public class CombatResult
{
    public int EnemiesBeforeFight { get; set; }
    public int EnemiesAfterFight { get; set; }
    public StoreSword? Loot { get; set; }
    public bool AllMonstersDefeated { get; set; }
    public string Message { get; set; } = "";
    public string CharacterName { get; set; } = "";
    public string MonsterName { get; set; } = "";
    public int CharacterHpAfterFight { get; set; }
    public int MonsterHpAfterFight { get; set; }
    public bool CharacterWon { get; set; }
    public int XpGained { get; set; }
    public int NewLevel { get; set; }
    public List<string> BattleLog { get; set; } = [];
}