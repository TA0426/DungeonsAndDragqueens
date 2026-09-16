namespace OppgaveUkeEnModul3.Core;

public class CharacterMonsterProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CharacterId { get; set; }

    public Guid MonsterId { get; set; }

    public int RemainingQuantity { get; set; }
}