namespace DungeonsAndDragqueens.Core;

public class StoreCharacter
{
    public string UserId { get; set; } = "";
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string CharacterClass { get; set; } = "Warrior";
    public bool IsHardcore { get; set; } = true;
    public int Hp { get; set; } = 300;
    public int MaxHp { get; set; } = 300;
    public int Damage { get; set; } = 10;
    public int Level { get; set; } = 1;
    public int XP { get; set; } = 0;
    public Guid? SwordId { get; set; }
    public int SwordLevel { get; set; } = 1;
    public Guid? BootsId { get; set; }
    public Guid? ChestId { get; set; }
    public Guid? LegsId { get; set; }
    public Guid? GlovesId { get; set; }
    public Guid? HelmetId { get; set; }
    public int BootsLevel { get; set; } = 1;
    public int ChestLevel { get; set; } = 1;
    public int LegsLevel { get; set; } = 1;
    public int GlovesLevel { get; set; } = 1;
    public int HelmetLevel { get; set; } = 1;
    public int Round { get; set; } = 0;
    public int LastCampRound { get; set; } = 0;

}
