namespace DungeonsAndDragqueens.Core;

public class CharacterLootOffer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public string ChoicesJson { get; set; } = "[]";
    public bool IsClaimed { get; set; }
}
