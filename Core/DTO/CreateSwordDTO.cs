namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;


public record CreateSwordDTO
{
   [Required]
   [MinLength(2)]
   [MaxLength(50)]
   public string Name { get; set; } = "";
   [Range(0, int.MaxValue)]
   public int Damage { get; set; }
   [Required]
   [MinLength(2)]
   [MaxLength(200)] public string Description { get; set; } = "";
}
