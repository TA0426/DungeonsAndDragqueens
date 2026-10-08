namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record CharacterResponse(
    Guid Id,
    string Name,
    int Hp,
    int Damage,
    int Level);
