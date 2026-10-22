namespace DungeonsAndDragqueens.Core;

public class StoreArmor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Slot { get; set; } = "";
    public int Defense { get; set; }
    public int SwordDamageBonus { get; set; }
    public string Description { get; set; } = "";
    public string AsciiArt { get; set; } = "";
}
