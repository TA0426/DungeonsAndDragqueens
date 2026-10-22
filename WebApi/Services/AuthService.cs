using DungeonsAndDragqueens.Core.Models;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DungeonsAndDragqueens.WebApi.Services;

public class AuthService(
    StoreMonstersContext database,
    IPasswordHasher<PlayerAccount> passwordHasher)
{
    public const int AutoFightCost = 50;
    public const int AutoCampCost = 100;

    public async Task<PlayerFeatures> GetFeaturesAsync(Guid accountId)
    {
        var account = await database.PlayerAccounts.FindAsync(accountId)
            ?? throw new KeyNotFoundException("Account not found.");

        return ToPlayerFeatures(account);
    }

    public async Task<PlayerFeatures> PurchaseAutoFeatureAsync(
        Guid accountId,
        string feature)
    {
        var account = await database.PlayerAccounts.FindAsync(accountId)
            ?? throw new KeyNotFoundException("Account not found.");

        var cost = feature switch
        {
            "auto-fight" => AutoFightCost,
            "auto-camp" => AutoCampCost,
            _ => throw new ArgumentException("Unknown auto feature.")
        };

        if (feature == "auto-fight" && account.HasAutoFight ||
            feature == "auto-camp" && account.HasAutoCamp)
            throw new InvalidOperationException("You already own this feature.");

        if (account.Gold < cost)
            throw new InvalidOperationException("Not enough gold.");

        account.Gold -= cost;
        if (feature == "auto-fight") account.HasAutoFight = true;
        else account.HasAutoCamp = true;
        await database.SaveChangesAsync();

        return ToPlayerFeatures(account);
    }

    private static PlayerFeatures ToPlayerFeatures(PlayerAccount account) =>
        new(account.Gold, account.HasAutoFight, account.HasAutoCamp,
            AutoFightCost, AutoCampCost);

    public async Task<bool> RegisterAsync(string userName, string password)
    {
        var normalizedUserName = userName.Trim().ToUpperInvariant();
        if (await database.PlayerAccounts.AnyAsync(account =>
                account.NormalizedUserName == normalizedUserName))
        {
            return false;
        }

        var account = new PlayerAccount
        {
            UserName = userName.Trim(),
            NormalizedUserName = normalizedUserName
        };
        account.PasswordHash = passwordHasher.HashPassword(account, password);

        database.PlayerAccounts.Add(account);
        await database.SaveChangesAsync();
        return true;
    }

    public async Task<PlayerAccount?> VerifyCredentialsAsync(
        string userName,
        string password)
    {
        var normalizedUserName = userName.Trim().ToUpperInvariant();
        var account = await database.PlayerAccounts.SingleOrDefaultAsync(
            candidate => candidate.NormalizedUserName == normalizedUserName);

        if (account is null)
            return null;

        var verification = passwordHasher.VerifyHashedPassword(
            account,
            account.PasswordHash,
            password);

        if (verification == PasswordVerificationResult.Failed)
            return null;

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            account.PasswordHash = passwordHasher.HashPassword(account, password);
            await database.SaveChangesAsync();
        }

        return account;
    }
}

public record PlayerFeatures(
    int Gold,
    bool HasAutoFight,
    bool HasAutoCamp,
    int AutoFightCost,
    int AutoCampCost);
