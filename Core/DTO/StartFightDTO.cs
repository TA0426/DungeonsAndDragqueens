namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;


public record StartFightDTO
{
   [Required]
   public Guid? CharacterId { get; set; }

   [Required]
   public Guid? MonsterId { get; set; }
}