namespace OppgaveUkeEnModul3.Core.Models;

namespace OppgaveUkeEnModul3.Core.Models;

public class Character(guid characterGuid, string characterName, int characterHp, int characterDamage, int characterLevel)
{
    public Guid CharacterGuid { get; init; } = characterGuid;
    public string CharacterName { get; set; } = characterName;
    public int CharacterHP { get; set; } = characterHp;
    public int CharacterDamage { get; set; } = characterDamage;
    public int CharacterLevel { get; set; } = characterLevel;
}