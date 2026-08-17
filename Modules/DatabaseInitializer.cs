using ksimb_membership.Modules.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ksimb_membership.Modules;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services)
    {
        await using var scope =
            services.CreateAsyncScope();

        var contextFactory =
            scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

        var configuration =
            scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

        var passwordHasher =
            scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<SecuritySettings>>();

        await using var context =
            await contextFactory.CreateDbContextAsync();

        // Ako već koristiš EnsureCreated/Migrate:
        await context.Database.MigrateAsync();

        var exists = await context.SecuritySettings
            .AnyAsync();

        if (exists)
            return;

        var secret = configuration["Security:AdminSecret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Security:AdminSecret is not configured.");
        }

        var settings = new SecuritySettings
        {
            Id = Guid.NewGuid(),
            AdminSecretHash = passwordHasher.HashPassword(
                null!,
                secret)
        };

        context.SecuritySettings.Add(settings);

        await context.SaveChangesAsync();
    }
}