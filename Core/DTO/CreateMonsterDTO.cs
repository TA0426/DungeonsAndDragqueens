namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record CreateMonsterDTO
{
   [Required]
   [MinLength(2)]
   [MaxLength(50)] public string Name { get; set; } = "";
   [Range(1, int.MaxValue)] public int Quantity { get; set; }
   [Required]
   [MinLength(2)]
   [MaxLength(50)] public string TypeOfMonster { get; set; } = "";
   [Range(1, int.MaxValue)] public int Hp { get; set; }
   [Range(0, int.MaxValue)] public int Damage { get; set; }
   [Range(0, int.MaxValue)] public int XPReward { get; set; }
   [Required]
   [MinLength(2)]
   [MaxLength(200)] public string Description { get; set; } = "";
}
