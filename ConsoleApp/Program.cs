using System.Net.Http.Headers;
using System.Net.Http.Json;

using var client = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5209")
};

while (true)
{

    Console.Write("Skriv inn brukernavn: ");
    var username = Console.ReadLine() ?? "";

    Console.Write("Skriv inn passord: ");
    var password = Console.ReadLine() ?? "";

    var loginResponse = await client.PostAsJsonAsync(
        "/auth/login",
        new { username, password });

    if (!loginResponse.IsSuccessStatusCode)
    {
        var error = await loginResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"Innlogging mislyktes. Status: {(int)loginResponse.StatusCode}");

        Console.WriteLine(error);

        continue;
    }

    var login = await loginResponse.Content
        .ReadFromJsonAsync<LoginResponse>();

    if (login is null)
    {
        Console.WriteLine("Kunne ikke lese token.");
        continue;
    }

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", login.Token);

    Console.WriteLine("Innlogging vellykket.");
    Console.WriteLine($"JWT-token: {login.Token}");

    Console.WriteLine("Dungeons and Dragqueens: If you die, you die!");
    Console.WriteLine();
    Console.WriteLine("1. Add monster");
    Console.WriteLine("2. Add character");
    Console.WriteLine("3. Add sword");
    Console.WriteLine("4. Start fight (permadeath)");
    Console.WriteLine("5. Go to camp");
    Console.WriteLine("0. Exit");
    Console.WriteLine("Choose: ");

    var choice = Console.ReadKey();

    if (choice.KeyChar == '0')
        break;

    if (choice.KeyChar == '1')
    {
        Console.WriteLine();
        Console.WriteLine("Monster name: ");
        var name = Console.ReadLine() ?? "";

        Console.WriteLine("Quantity: ");
        var quantity = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Type: ");
        var typeOfMonster = Console.ReadLine() ?? "";

        Console.WriteLine("HP: ");
        var hp = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Damage: ");
        var damage = int.Parse(Console.ReadLine()!);

        Console.WriteLine("XP: ");
        var xp = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Description: ");
        var description = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync(
            "/StoreMonsters",
            new
            {
                name,
                quantity,
                typeOfMonster,
                hp,
                damage,
                XPReward = xp,
                description
            });

        var message = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)response.StatusCode}: {message}");

            continue;
        }

        Console.WriteLine("Monster created:");
        Console.WriteLine(message);
    }
    else if (choice.KeyChar == '2')
    {
        Console.WriteLine();
        Console.WriteLine("Character name: ");
        var name = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync("/StoreCharacters", new
        {
            name
        });
        var message = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)response.StatusCode}: {message}");

            continue;
        }

        Console.WriteLine("Character created:");
        Console.WriteLine(message);
    }
    else if (choice.KeyChar == '3')
    {
        Console.WriteLine();
        Console.WriteLine("Sword name: ");
        var name = Console.ReadLine() ?? "";

        Console.WriteLine("Damage: ");
        var damage = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Description: ");
        var description = Console.ReadLine() ?? "";

        var response = await client.PostAsJsonAsync("/StoreSwords", new
        {
            name,
            damage,
            description
        });

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else if (choice.KeyChar == '4')
    {
        Console.WriteLine();

        var characterResponse =
            await client.GetAsync("/StoreCharacters");

        if (!characterResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)characterResponse.StatusCode}: " +
                await characterResponse.Content.ReadAsStringAsync());

            continue;
        }

        var characters =
            await characterResponse.Content.ReadFromJsonAsync<
                List<CharacterForConsole>>();

        var monsterResponse =
            await client.GetAsync("/StoreMonsters");

        if (!monsterResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)monsterResponse.StatusCode}: " +
                await monsterResponse.Content.ReadAsStringAsync());

            continue;
        }

        var monsters =
            await monsterResponse.Content.ReadFromJsonAsync<
                List<MonsterForConsole>>();

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

        var fightResponse =
            await client.PostAsJsonAsync("/Fight", body);

        if (!fightResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)fightResponse.StatusCode}: " +
                await fightResponse.Content.ReadAsStringAsync());

            continue;
        }

        var fightResult =
            await fightResponse.Content.ReadFromJsonAsync<
                CombatResultForConsole>();

        if (fightResult is null)
        {
            Console.WriteLine("Kunne ikke lese fight-resultatet.");
            continue;
        }

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

        Console.WriteLine(
            fightResult.CharacterWon
                ? "You won!"
                : "You lost!");

        var loot = fightResult.Loot;

        if (loot is not null)
        {
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
    else if (choice.KeyChar == '5')
    {
        Console.WriteLine();
        var response = await client.GetAsync("/StoreCharacters");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)response.StatusCode}: " +
                await response.Content.ReadAsStringAsync());

            continue;
        }

        var characters =
            await response.Content.ReadFromJsonAsync<
                List<CharacterForConsole>>();



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

        var campResponse = await client.PostAsync(
            $"/StoreCharacters/{characterId}/camp",
            null);

        var message =
            await campResponse.Content.ReadAsStringAsync();

        if (!campResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Feil {(int)campResponse.StatusCode}: {message}");

            continue;
        }

        Console.WriteLine(message);
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

public record LoginResponse(string Token);