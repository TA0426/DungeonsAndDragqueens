namespace DungeonsAndDragqueens.Core;

public class CharacterDungeonProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public string DungeonId { get; set; } = "";
    public int CurrentWave { get; set; } = 1;
    public bool IsCompleted { get; set; }
    public int ClearCount { get; set; }
}
