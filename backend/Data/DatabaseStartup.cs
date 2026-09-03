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
                    await EnsureHeaderLinksTableAsync(db);
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

    private static async Task EnsureHeaderLinksTableAsync(AppDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS `header_links` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `Label` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Path` varchar(240) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `SortOrder` int NOT NULL,
              `Visible` tinyint(1) NOT NULL,
              `IsCta` tinyint(1) NOT NULL,
              `CreatedAtUtc` datetime(6) NOT NULL,
              PRIMARY KEY (`Id`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            """);
    }
}
