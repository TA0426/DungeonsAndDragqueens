namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record StartDungeonFightDTO
{
    [Required]
    public Guid? CharacterId { get; set; }

    [Required]
    public string DungeonId { get; set; } = "";
}
