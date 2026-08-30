using Microsoft.EntityFrameworkCore;

namespace SaraRose.Api.Data;

public static class DatabaseStartup
{
    public static async Task InitializeAsync(AppDbContext db, ILogger logger)
    {
        const int attempts = 20;
        for (var i = 1; i <= attempts; i++)
        {
            try
            {
                if (await db.Database.CanConnectAsync())
                {
                    await db.Database.EnsureCreatedAsync();
                    await DbSeeder.SeedAsync(db);
                    return;
                }
            }
            catch (Exception ex) when (i < attempts)
            {
                logger.LogWarning(
                    "MySQL is not ready yet (attempt {Attempt}/{Attempts}): {Message}",
                    i,
                    attempts,
                    ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new InvalidOperationException(
            "Cannot reach MySQL at 127.0.0.1:3306. Start the MySQL service and create the sararose database (see README.md).");
    }
}
