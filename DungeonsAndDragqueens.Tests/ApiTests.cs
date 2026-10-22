using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using DungeonsAndDragqueens.Core;


namespace DungeonsAndDragqueens.Tests;

public class ApiTests
{
    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var registration = await client.PostAsJsonAsync(
            "/auth/register",
            new
            {
                userName = $"player_{Guid.NewGuid():N}"[..15],
                password = "a-strong-test-password"
            });
        registration.EnsureSuccessStatusCode();

        var login = await registration.Content
            .ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                login!.Token);
        return client;
    }

    [Fact]
    public async Task GetCharacters_ReturnsOk()
    {
        await using var factory =
            new WebApplicationFactory<Program>();

        using var client = await CreateAuthenticatedClientAsync(factory);

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

        using var client = await CreateAuthenticatedClientAsync(factory);

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

        using var client = await CreateAuthenticatedClientAsync(factory);

        var characterResponse = await client.PostAsJsonAsync(
            "/StoreCharacters",
            new
            {
                name = "Anna"
            });

        var character =
            await characterResponse.Content.ReadFromJsonAsync<StoreCharacter>();

        var monsters = await client.GetFromJsonAsync<List<StoreMonster>>(
            "/StoreMonsters?page=1&pageSize=100");
        var monster = monsters!.FirstOrDefault(candidate => candidate.Quantity > 0);
        Assert.NotNull(monster);

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

public record LoginResponse(string Token, string UserName);
