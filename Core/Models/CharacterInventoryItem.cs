namespace DungeonsAndDragqueens.Core;

public class CharacterInventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Guid? SwordId { get; set; }
    public Guid? ArmorId { get; set; }
    public int Level { get; set; } = 1;
    public int Quantity { get; set; } = 1;
}
