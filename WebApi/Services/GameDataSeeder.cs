using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DungeonsAndDragqueens.WebApi.Services;

public static class GameDataSeeder
{
    public static async Task SeedAsync(StoreMonstersContext database)
    {
        if (!await database.StoreMonsters.AnyAsync())
        {
            database.StoreMonsters.AddRange(
                new StoreMonster
                {
                    Name = "Cave Slime",
                    Quantity = 5,
                    TypeOfMonster = "Slime",
                    Hp = 25,
                    Damage = 4,
                    XPReward = 20,
                    Description = "A wobbly beginner's foe."
                },
                new StoreMonster
                {
                    Name = "Goblin Scout",
                    Quantity = 3,
                    TypeOfMonster = "Goblin",
                    Hp = 55,
                    Damage = 12,
                    XPReward = 45,
                    Description = "Quick, noisy, and fond of shiny things."
                },
                new StoreMonster
                {
                    Name = "Crypt Wraith",
                    Quantity = 2,
                    TypeOfMonster = "Undead",
                    Hp = 100,
                    Damage = 22,
                    XPReward = 90,
                    Description = "A dangerous spirit from the old catacombs."
                });
        }

        if (!await database.Swords.AnyAsync())
        {
            database.Swords.AddRange(
                new StoreSword
                {
                    Name = "Tin Dagger",
                    Damage = 8,
                    Description = "A modest blade for a new adventurer."
                },
                new StoreSword
                {
                    Name = "Moonsteel Rapier",
                    Damage = 20,
                    Description = "A light blade that catches the moonlight."
                });
        }

        await database.SaveChangesAsync();
    }
}
