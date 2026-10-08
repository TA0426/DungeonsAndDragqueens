namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record CreateCharacterDTO
{
   [Required]
   [MinLength(2)]
   [MaxLength(50)]
   public string Name { get; set; } = "";
}
