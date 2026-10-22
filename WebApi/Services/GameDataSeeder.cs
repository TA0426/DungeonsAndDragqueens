using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DungeonsAndDragqueens.WebApi.Services;

public static class GameDataSeeder
{
    private static readonly (string Name, int Damage, string Description)[] SwordCatalog =
    [
        ("Tin Dagger", 8, "A modest blade for a new adventurer."),
        ("Moonsteel Rapier", 20, "A light blade that catches the moonlight."),
        ("Rose Thorn", 14, "A slim, rose-hilted dueling sword."),
        ("Glitterfang", 18, "A jeweled blade that flashes in battle."),
        ("Queensguard Saber", 24, "A curved saber fit for royalty."),
        ("Rainbow Edge", 27, "A prismatic blade leaving colorful trails."),
        ("Ruby Greatsword", 32, "A heavy sword set with a red gem."),
        ("Starfall Blade", 36, "A sword forged from a fallen star."),
        ("Midnight Stiletto", 22, "A dark, precise blade for quick strikes."),
        ("Dragonheart Claymore", 42, "A mighty blade warmed by dragonfire."),
        ("Crystal Spellblade", 30, "A crystal-edged sword humming with magic."),
        ("Eclipse Katana", 39, "A sharp blade forged beneath an eclipse.")
    ];

    private static readonly (string Name, string Slot, int Defense, int Bonus)[] ArmorCatalog =
    [
        ("Leather Boots", "Boots", 2, 0), ("Swiftstep Boots", "Boots", 3, 2), ("Iron Greaves", "Boots", 5, 0),
        ("Silk Duelist Coat", "Chest", 3, 4), ("Chainmail Bodice", "Chest", 7, 0), ("Dragonplate Cuirass", "Chest", 12, 2),
        ("Traveler Leggings", "Legs", 2, 0), ("Shadow Trousers", "Legs", 4, 3), ("Runed Legplates", "Legs", 7, 1),
        ("Lace Gloves", "Gloves", 1, 3), ("Iron Gauntlets", "Gloves", 4, 1), ("Duelist Gloves", "Gloves", 2, 6),
        ("Feathered Mask", "Helmet", 2, 2), ("Steel Tiara", "Helmet", 5, 0), ("Dragon-Crest Helm", "Helmet", 9, 1)
    ];
    private static readonly MonsterSeed[] MonsterCatalog =
    [
        new("Cave Slime", 5, "Slime", 25, 4, 20, "A wobbly beginner's foe."),
        new("Goblin Scout", 3, "Goblin", 55, 12, 45, "Quick, noisy, and fond of shiny things."),
        new("Crypt Wraith", 2, "Undead", 100, 22, 90, "A dangerous spirit from the old catacombs."),
        new("Marsh Ghost", 2, "Ghost", 75, 14, 60, "A pale spirit drifting over the marsh."),
        new("Grave Ghoul", 2, "Ghoul", 85, 18, 70, "A hungry creature lurking by old graves."),
        new("Restless Undead", 2, "Undead", 90, 18, 75, "A fallen adventurer who refuses to rest."),
        new("Bone Skeleton", 3, "Skeleton", 70, 12, 55, "A rattling skeleton with a rusty blade."),
        new("Rotting Zombie", 3, "Zombie", 80, 12, 55, "Slow, stubborn, and very hard to discourage."),
        new("Vampire Countess", 1, "Vampire", 180, 30, 140, "A noble night-stalker with a taste for drama."),
        new("Lich Queen", 1, "Lich", 260, 42, 220, "An ancient spellcaster crowned in bone."),
        new("Orc Champion", 2, "Orc", 130, 24, 100, "A battle-tested orc with a fearsome tusk-grin."),
        new("Ogre Bouncer", 2, "Ogre", 200, 32, 160, "A huge bruiser who guards the dungeon gates."),
        new("Bridge Troll", 2, "Troll", 210, 28, 170, "A stubborn troll demanding a toll."),
        new("Granite Golem", 1, "Golem", 280, 38, 220, "A stone guardian brought to life by magic."),
        new("Dungeon Wizard", 2, "Wizard", 120, 24, 120, "A robe-clad wizard with glittering spells."),
        new("Apprentice Mage", 2, "Mage", 100, 18, 90, "A young mage whose spells are less than predictable."),
        new("Shadow Warlock", 1, "Warlock", 170, 32, 160, "A warlock whose power comes from a dark pact."),
        new("Bog Witch", 2, "Witch", 130, 24, 120, "A crafty witch brewing trouble in the bog."),
        new("Ruby Dragon", 1, "Dragon", 500, 65, 450, "A mighty dragon guarding a hoard of treasure."),
        new("Pit Demon", 1, "Demon", 300, 48, 300, "A horned demon escaped from the lower realm."),
        new("Mischief Imp", 3, "Imp", 80, 15, 70, "A small fiend with an oversized attitude."),
        new("Fire Elemental", 1, "Fire Elemental", 180, 35, 180, "A blazing spirit of living flame."),
        new("Frost Elemental", 1, "Ice Elemental", 180, 30, 180, "A crystal-cold spirit that chills the air."),
        new("Dusty Mummy", 2, "Mummy", 120, 22, 110, "An ancient ruler wrapped in enchanted bandages."),
        new("One-Eyed Cyclops", 1, "Cyclops", 240, 40, 200, "A towering giant with a single glaring eye."),
        new("Labyrinth Minotaur", 1, "Minotaur", 220, 40, 190, "A horned guardian that knows every maze."),
        new("Many-Headed Hydra", 1, "Hydra", 500, 55, 400, "A many-headed beast guarding a flooded cavern."),
        new("Abyssal Kraken", 1, "Kraken", 600, 70, 500, "A colossal sea monster with grasping tentacles."),
        new("Castle Gargoyle", 2, "Gargoyle", 210, 38, 180, "A stone-winged sentinel that wakes at night."),
        new("Dawn Phoenix", 1, "Phoenix", 430, 55, 400, "A brilliant firebird reborn from its ashes."),
        new("Storm Elemental", 1, "Elemental", 200, 30, 190, "A swirling spirit of wind and lightning."),
        new("Cave Spider", 3, "Spider", 65, 14, 50, "A venomous spider spinning webs in dark tunnels."),
        new("Moonlit Werewolf", 1, "Werewolf", 180, 34, 160, "A fierce shapeshifter under the full moon."),
        new("Grey Wolf", 3, "Wolf", 60, 11, 45, "A keen-eyed wolf hunting in the dungeon depths."),
        new("Vampire Bat", 3, "Bat", 35, 8, 25, "A swift bat swooping from the rafters."),
        new("Jade Serpent", 2, "Serpent", 95, 18, 75, "A long-scaled serpent guarding a forgotten shrine."),
        new("Cinder Snake", 3, "Snake", 45, 9, 35, "A small but venomous snake among the stones."),
        new("Fallen Knight", 2, "Knight", 190, 33, 160, "An armored knight bound to an ancient oath."),
        new("Mimic Chest", 2, "Mimic", 130, 25, 120, "A treasure chest with a very unexpected bite."),
        new("Cliff Harpy", 2, "Harpy", 140, 24, 130, "A sharp-clawed flier with a piercing cry."),
        new("Wild Beast", 2, "Beast", 170, 30, 150, "A powerful creature prowling the wild tunnels."),
        new("Unknown Monster", 2, "Monster", 100, 20, 80, "A mysterious creature with an unfamiliar shape.")
    ];

    public static async Task SeedAsync(StoreMonstersContext database)
    {
        var existingNames = (await database.StoreMonsters
                .Select(monster => monster.Name)
                .ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingMonsters = MonsterCatalog
            .Where(monster => existingNames.Add(monster.Name))
            .Select(monster => new StoreMonster
            {
                Name = monster.Name,
                Quantity = monster.Quantity,
                TypeOfMonster = monster.Type,
                Hp = monster.Hp,
                Damage = monster.Damage,
                XPReward = monster.Xp,
                Description = monster.Description
            });

        database.StoreMonsters.AddRange(missingMonsters);

        var existingSwords = await database.Swords.ToListAsync();
        var swordNames = existingSwords.Select(sword => sword.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in SwordCatalog)
        {
            var art = SwordArt(item.Name);
            var existing = existingSwords.FirstOrDefault(sword => string.Equals(sword.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            if (existing is not null) { if (string.IsNullOrWhiteSpace(existing.AsciiArt)) existing.AsciiArt = art; continue; }
            if (swordNames.Add(item.Name)) database.Swords.Add(new StoreSword { Name = item.Name, Damage = item.Damage, Description = item.Description, AsciiArt = art });
        }

        var armorNames = (await database.Armors.Select(item => item.Name).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in ArmorCatalog)
            if (armorNames.Add(item.Name)) database.Armors.Add(new StoreArmor { Name = item.Name, Slot = item.Slot, Defense = item.Defense, SwordDamageBonus = item.Bonus, Description = $"{item.Slot} armor piece.", AsciiArt = ArmorArt(item.Slot, item.Name) });

        await database.SaveChangesAsync();
    }

    private static string SwordArt(string name)
    {
        var initials = string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0]));
        return $"       /\\\n      /  \\\n     / {initials} \\\n    /______\\\n       ||\n      (==)\n       ||";
    }

    private static string ArmorArt(string slot, string name)
    {
        var mark = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0][..Math.Min(3, name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Length)].ToUpperInvariant();
        return slot switch
        {
            "Boots" => $"  _{mark}_  \n /    \\\n|______|",
            "Chest" => $" .-{mark}-.\n/|     |\\\n |_____|",
            "Legs" => $"  {mark}  \n | | |\n |_|_|",
            "Gloves" => $" _{mark}_\n/     \\\n\\_____/",
            _ => $" .-{mark}-.\n |(   )|\n  \\___/"
        };
    }

    private sealed record MonsterSeed(
        string Name,
        int Quantity,
        string Type,
        int Hp,
        int Damage,
        int Xp,
        string Description);
}
