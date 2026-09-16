using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using OppgaveUkeEnModul3.Core;


namespace Calculator.Tests;

public class ApiTests
{
    [Fact]
    public async Task GetCharacters_ReturnsOk()
    {
        await using var factory =
            new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var response =
            await client.GetAsync("/StoreCharacters");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task PostCharacter_ReturnsCreated()
    {
        await using var factory =
            new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/StoreCharacters",
            new
            {
                name = "Anna"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task PostFight_ReturnsOk()
    {
        await using var factory =
            new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var characterResponse = await client.PostAsJsonAsync(
            "/StoreCharacters",
            new
            {
                name = "Anna"
            });

        var character =
            await characterResponse.Content.ReadFromJsonAsync<StoreCharacter>();

        var monsterResponse = await client.PostAsJsonAsync(
            "/StoreMonsters",
            new
            {
                name = "Weak Monster",
                quantity = 1,
                typeOfMonster = "Goblin",
                hp = 10,
                damage = 1,
                xpReward = 10,
                description = "A weak test monster"
            });

        var monster =
            await monsterResponse.Content.ReadFromJsonAsync<StoreMonster>();

        var fightResponse = await client.PostAsJsonAsync(
            "/Fight",
            new
            {
                characterId = character!.Id,
                monsterId = monster!.Id
            });

        Assert.Equal(
            HttpStatusCode.OK,
            fightResponse.StatusCode);
    }
}