using DungeonsAndDragqueens.Core;

namespace DungeonsAndDragqueens.WebApi.Services;

public static class DungeonCatalog
{
    public static readonly IReadOnlyList<DungeonDefinition> All =
    [
        new("echo-caverns", "Echo Caverns", "The Verdant Deep", "Easy", 1, 8,
            "A first expedition through damp caves and old tunnels.",
            [["Cave Slime", "Vampire Bat", "Grey Wolf"], ["Goblin Scout", "Cave Spider", "Grave Ghoul"], ["Bridge Troll", "Ogre Bouncer", "Granite Golem"]], "Granite Golem",
            "     /\\      \n    /  \\     \n   /____\\    \n  |  []  |    \n  |______|"),
        new("sunken-court", "Sunken Court", "The Drowned Kingdom", "Moderate", 65, 20,
            "Flooded halls where restless nobles guard a lost throne.",
            [["Marsh Ghost", "Bone Skeleton", "Jade Serpent"], ["Dusty Mummy", "Crypt Wraith", "Restless Undead"], ["Vampire Countess", "Many-Headed Hydra", "Abyssal Kraken"]], "Abyssal Kraken",
            " ~ ~ ~ ~ ~  \n  _/^^^^\\_   \n /  (oo)  \\  \n|___/||\\___| \n   /_||_\\"),
        new("ember-wastes", "Ember Wastes", "The Ashen Expanse", "Hard", 150, 30,
            "Volcanic badlands patrolled by fire and fang.",
            [["Cinder Snake", "Orc Champion", "Fire Elemental"], ["Pit Demon", "One-Eyed Cyclops", "Dawn Phoenix"], ["Ruby Dragon", "Shadow Warlock", "Granite Golem"]], "Ruby Dragon",
            "     /\\   /\\    \n  __/  \\_/  \\__ \n /   (  *  )   \\ \n|____/#####\\____|\n     /_/ \\_\\"),
        new("shadow-citadel", "Shadow Citadel", "The Umbral Reach", "Very hard", 250, 40,
            "A haunted fortress where the dead still serve their queen.",
            [["Rotting Zombie", "Fallen Knight", "Castle Gargoyle"], ["Dungeon Wizard", "Shadow Warlock", "Vampire Countess"], ["Lich Queen", "Pit Demon", "Abyssal Kraken"]], "Lich Queen",
            "  |>>>|<<<|   \n  |   ___   |  \n  |  (x x)  |  \n  |___/###\\_|\n  /_________\\"),
        new("celestial-rift", "Celestial Rift", "The Astral Crown", "Nightmare", 375, 42,
            "A tear between worlds filled with ancient, unstable power.",
            [["Storm Elemental", "Frost Elemental", "Cliff Harpy"], ["Labyrinth Minotaur", "Dawn Phoenix", "Castle Gargoyle"], ["Ruby Dragon", "Many-Headed Hydra", "Abyssal Kraken"]], "Pit Demon",
            "      . * .      \n   .  /|\\  .    \n  ---( * )---    \n   ' /|\\ '      \n     / \\        ")
    ];

    public const string EndlessId = "endless-descent";

    public static readonly DungeonDefinition Endless = new(
        EndlessId, "Endless Descent", "Beyond the Astral Crown", "Endless", 280, 42,
        "An ever-deepening dungeon. A boss waits every ten waves.",
        [All.SelectMany(dungeon => dungeon.EnemyWaves.SelectMany(wave => wave)).Distinct().ToArray()],
        "Endless Champion",
        "       |>>>|       \n     __|___|__     \n    /  ?????  \\    \n   /___/###\\___\\   \n       /   \\");

    public static DungeonDefinition? Find(string id) =>
        id == EndlessId ? Endless : All.FirstOrDefault(item => item.Id == id);

    public static int IndexOf(string id)
    {
        for (var index = 0; index < All.Count; index++)
            if (All[index].Id == id) return index;
        return -1;
    }
}
