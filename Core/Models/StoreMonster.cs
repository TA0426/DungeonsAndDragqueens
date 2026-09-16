namespace OppgaveUkeEnModul3.Core;

public class StoreMonster
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public string TypeOfMonster { get; set; } = "";
    public int Hp { get; set; }
    public int Damage { get; set; }
    public int XPReward { get; set; }
    public string Description { get; set; } = "";
}