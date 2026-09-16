namespace OppgaveUkeEnModul3.Core.Models;

public class Monsters(guid monsterid, string monstername, int quantity, string typeOfMonster, int monsterHp, int monsterDamage)
{
    public Guid MonsterId { get; init; } = monsterId;
    public string MonsterName { get; set; } = monsterName;
    public int Quantity { get; set; } = quantity;
    public string TypeOfMonster { get; set; } = typeOfMonster;
    public int MonsterHP { get; set; } = monsterHp;
    public int MonsterDamage { get; set; } = monsterDamage;
    public int XP { get; set; } = xp;
    public string Description { get; set; } = description;
}