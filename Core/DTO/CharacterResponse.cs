namespace DungeonsAndDragqueens.Core;

using System.ComponentModel.DataAnnotations;

public record CharacterResponse(
    Guid Id,
    string Name,
    int Hp,
    int MaxHp,
    int Damage,
    int Level,
    Guid? SwordId,
    bool IsHardcore,
    string CharacterClass,
    Guid? BootsId,
    Guid? ChestId,
    Guid? LegsId,
    Guid? GlovesId,
    Guid? HelmetId,
    int SwordLevel,
    int BootsLevel,
    int ChestLevel,
    int LegsLevel,
    int GlovesLevel,
    int HelmetLevel);
