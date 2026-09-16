namespace OppgaveUkeEnModul3.Core;

using System.ComponentModel.DataAnnotations;

public record CreateMonsterDTO(
   [Required] string Name,
   [Range(1, int.MaxValue)] int Quantity,
   [Required] string TypeOfMonster,
   [Range(1, int.MaxValue)] int Hp,
   [Range(0, int.MaxValue)] int Damage,
   [Range(0, int.MaxValue)] int XPReward,
   [Required] string Description);