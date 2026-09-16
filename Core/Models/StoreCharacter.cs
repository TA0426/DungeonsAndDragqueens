namespace OppgaveUkeEnModul3.Core;

public class StoreCharacter
{
    public string UserId { get; set; } = "";
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Hp { get; set; } = 300;
    public int MaxHp { get; set; } = 300;
    public int Damage { get; set; } = 10;
    public int Level { get; set; } = 1;
    public int XP { get; set; } = 0;
    public Guid? SwordId { get; set; }
    public int Round { get; set; } = 0;
    public int LastCampRound { get; set; } = 0;
}