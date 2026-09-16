using System.Net.Http.Json;

using var client = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5209")
};

while (true)
{
    Console.WriteLine("Dungeons and Dragqueens: If you die, you die!");
    Console.WriteLine();
    Console.WriteLine("1. Add monster");
    Console.WriteLine("2. Add character");
    Console.WriteLine("3. Add sword");
    Console.WriteLine("4. Start fight (permadeath)");
    Console.WriteLine("5. Go to camp");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    var choice = Console.ReadLine();

    if (choice == "0")
        break;

    if (choice == "1")
    {
        Console.Write("Monster name: ");
        var name = Console.ReadLine() ?? "";

        Console.Write("Quantity: ");
        var quantity = int.Parse(Console.ReadLine()!);

        Console.Write("Type: ");
        var typeOfMonster = Console.ReadLine() ?? "";

        Console.Write("HP: ");
        var hp = int.Parse(Console.ReadLine()!);

        Console.Write("Damage: ");
        var damage = int.Parse(Console.ReadLine()!);

        Console.Write("XP: ");
        var xp = int.Parse(Console.ReadLine()!);

        Console.Write("Description: ");
        var description = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync("/StoreMonsters", new
        {
            name,
            quantity,
            typeOfMonster,
            hp,
            damage,
            XPReward = xp,
            description
        });

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else if (choice == "2")
    {
        Console.Write("Character name: ");
        var name = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync("/StoreCharacters", new
        {
            name
        });

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else if (choice == "3")
    {
        Console.Write("Sword name: ");
        var name = Console.ReadLine() ?? "";

        Console.Write("Damage: ");
        var damage = int.Parse(Console.ReadLine()!);

        Console.Write("Description: ");
        var description = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync("/StoreSwords", new
        {
            name,
            damage,
            description
        });

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else if (choice == "4")
    {
        var characters =
            await client.GetFromJsonAsync<List<CharacterForConsole>>(
                "/StoreCharacters");

        var monsters =
            await client.GetFromJsonAsync<List<MonsterForConsole>>(
                "/StoreMonsters");

        if (characters is null || characters.Count == 0)
        {
            Console.WriteLine("No characters found.");
            continue;
        }

        if (monsters is null || monsters.Count == 0)
        {
            Console.WriteLine("No monsters found.");
            continue;
        }

        Console.WriteLine("Choose character:");

        for (int i = 0; i < characters.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {characters[i].Name} - HP: {characters[i].Hp}");
        }

        var characterChoice = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Choose monster:");

        for (int i = 0; i < monsters.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {monsters[i].Name} - HP: {monsters[i].Hp}");
        }

        var monsterChoice = int.Parse(Console.ReadLine()!);

        var body = new
        {
            characterId = characters[characterChoice - 1].Id,
            monsterId = monsters[monsterChoice - 1].Id
        };

        var response = await client.PostAsJsonAsync("/Fight", body);

        var fightResult =
            await response.Content.ReadFromJsonAsync<CombatResultForConsole>();
        if (fightResult is not null)
        {
            foreach (var logLine in fightResult.BattleLog)
            {
                Console.WriteLine(logLine);
            }

            Console.WriteLine();
            Console.WriteLine("===== FIGHT RESULT =====");
            Console.WriteLine(
    $"Enemies before fight: {fightResult.EnemiesBeforeFight}");
            Console.WriteLine(
                $"Your HP: {fightResult.CharacterHpAfterFight}");
            Console.WriteLine(
                $"Monster HP: {fightResult.MonsterHpAfterFight}");
            Console.WriteLine(
                $"XP gained: {fightResult.XpGained}");
            Console.WriteLine(
                $"Your level: {fightResult.NewLevel}");


            Console.WriteLine(
                $"Enemies after fight: {fightResult.EnemiesAfterFight}");

            if (fightResult.CharacterWon)
            {
                Console.WriteLine("You won!");
            }
            else
            {
                Console.WriteLine("You lost!");
            }
        }
        if (fightResult.Loot is not null)
        {
            var loot = fightResult.Loot;

            Console.WriteLine(
                $"Loot found: {loot.Name} - Damage: {loot.Damage}");

            Console.Write("Equip this sword? (y/n): ");
            var equipChoice = Console.ReadLine();

            if (equipChoice?.ToLower() == "y")
            {
                var characterId =
                    characters[characterChoice - 1].Id;

                var equipResponse = await client.PutAsync(
                    $"/StoreCharacters/{characterId}" +
                    $"/equipment/{loot.Id}",
                    null);

                Console.WriteLine(
                    equipResponse.IsSuccessStatusCode
                        ? $"{loot.Name} equipped."
                        : "Could not equip sword.");
            }
        }
        else
        {
            Console.WriteLine("No loot this time.");
        }
    }
    else if (choice == "5")
    {
        var characters =
            await client.GetFromJsonAsync<List<CharacterForConsole>>(
                "/StoreCharacters");

        if (characters is null || characters.Count == 0)
        {
            Console.WriteLine("No characters found.");
            continue;
        }

        for (int i = 0; i < characters.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {characters[i].Name} - HP: {characters[i].Hp}");
        }

        Console.Write("Choose character: ");
        var characterChoice = int.Parse(Console.ReadLine()!);

        var characterId =
            characters[characterChoice - 1].Id;

        var response = await client.PostAsync(
            $"/StoreCharacters/{characterId}/camp",
            null);

        Console.WriteLine(
            await response.Content.ReadAsStringAsync());
    }
}





public record CharacterForConsole(
    Guid Id,
    string Name,
    int Hp,
    int Damage,
    int Level,
    int XP);

public record MonsterForConsole(
    Guid Id,
    string Name,
    int Hp,
    int Damage);

public record CombatResultForConsole(
    string CharacterName,
    string MonsterName,
    int CharacterHpAfterFight,
    int MonsterHpAfterFight,
    bool CharacterWon,
    int XpGained,
    int NewLevel,
    bool AllMonstersDefeated,
    string Message,
    LootSwordForConsole? Loot,
    int EnemiesBeforeFight,
    int EnemiesAfterFight,
    List<string> BattleLog);

public record LootSwordForConsole(
Guid Id,
string Name,
int Damage,
string Description);