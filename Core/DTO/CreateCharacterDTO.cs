namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record CreateCharacterDTO
{
   [Required]
   [MinLength(2)]
   [MaxLength(50)]
   public string Name { get; set; } = "";

   public bool IsHardcore { get; set; } = true;

   [Required]
   [RegularExpression("^(Warrior|Wizard|Warlock|Ranger|Assassin)$")]
   public string CharacterClass { get; set; } = "Warrior";
}
