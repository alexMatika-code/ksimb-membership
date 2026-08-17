using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ksimb_membership.Modules.Security;

public interface ISecurityService
{
    Task<bool> VerifyAdminSecret(string secret);
}

internal sealed class SecurityService(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    IPasswordHasher<SecuritySettings> passwordHasher)
    : ISecurityService
{
    public async Task<bool> VerifyAdminSecret(string secret)
    {
        await using var context =
            await contextFactory.CreateDbContextAsync();

        var settings = await context.SecuritySettings
            .SingleOrDefaultAsync();

        if (settings is null)
            return false;

        var result = passwordHasher.VerifyHashedPassword(
            settings,
            settings.AdminSecretHash,
            secret);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}