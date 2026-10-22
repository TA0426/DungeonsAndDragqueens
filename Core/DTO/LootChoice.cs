namespace DungeonsAndDragqueens.Core;

public record LootChoice(
    string Type,
    Guid ItemId,
    string Name,
    string Slot,
    int Level,
    int Damage,
    int Defense,
    int SwordDamageBonus,
    string AsciiArt);

public record ClaimLootDTO(int ChoiceIndex);

public record InventoryItemResponse(
    Guid Id,
    Guid? SwordId,
    Guid? ArmorId,
    string Name,
    string Type,
    string Slot,
    int Level,
    int Quantity,
    int Damage,
    int Defense,
    int SwordDamageBonus,
    string AsciiArt);
