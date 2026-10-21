using DungeonsAndDragqueens.Core.Models;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DungeonsAndDragqueens.WebApi.Services;

public class AuthService(
    StoreMonstersContext database,
    IPasswordHasher<PlayerAccount> passwordHasher)
{
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
