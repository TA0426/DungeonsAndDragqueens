namespace OppgaveUkeEnModul3.Core.Models;

public class Swords(guid swordid, string swordName, int quantity, string typeOfSword, int swordDamage, string description)
{
    public Guid SwordId { get; init; } = swordId;
    public string SwordName { get; set; } = swordName;
    public int Quantity { get; set; } = quantity;
    public string TypeOfSword { get; set; } = typeOfSword;
    public int SwordDamage { get; set; } = swordDamage;
    public string Description { get; set; } = description;
}
