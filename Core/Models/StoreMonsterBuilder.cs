using OppgaveUkeEnModul3.Core;

public class StoreMonsterBuilder
{
    public StoreMonster FromDto(CreateMonsterDTO dto) => new()
    {
        Name = dto.Name,
        Quantity = dto.Quantity,
        TypeOfMonster = dto.TypeOfMonster,
        Hp = dto.Hp,
        Damage = dto.Damage,
        XPReward = dto.XPReward,
        Description = dto.Description
    };
}
