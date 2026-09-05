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
                    await EnsureRegistrationsTableAsync(db);
                    await EnsureRegistrationColumnsAsync(db);
                    await EnsureUserMasterTableAsync(db);
                    await DropUserMasterRegistrationIdAsync(db);
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

    private static async Task EnsureRegistrationsTableAsync(AppDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS `registrations` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `FullName` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Email` varchar(160) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Phone` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Company` varchar(160) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
              `City` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
              `Role` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `UserType` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `PasswordHash` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `CreatedAtUtc` datetime(6) NOT NULL,
              PRIMARY KEY (`Id`),
              UNIQUE KEY `IX_registrations_Email` (`Email`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            """);
    }

    private static async Task EnsureRegistrationColumnsAsync(AppDbContext db)
    {
        await TryAddColumnAsync(db,
            "ALTER TABLE `registrations` ADD COLUMN `Role` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'User'");
        await TryAddColumnAsync(db,
            "ALTER TABLE `registrations` ADD COLUMN `UserType` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Customer'");
    }

    private static async Task EnsureUserMasterTableAsync(AppDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS `userMaster` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `Username` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Email` varchar(160) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `FullName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Phone` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Role` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `UserType` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `HashPassword` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `NormalPassword` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
              `Active` tinyint(1) NOT NULL DEFAULT 1,
              `CreatedAtUtc` datetime(6) NOT NULL,
              PRIMARY KEY (`Id`),
              UNIQUE KEY `IX_userMaster_Username` (`Username`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
            """);
    }

    private static async Task DropUserMasterRegistrationIdAsync(AppDbContext db)
    {
        await TryAddColumnAsync(db, "ALTER TABLE `userMaster` DROP COLUMN `RegistrationId`");
    }

    private static async Task TryAddColumnAsync(AppDbContext db, string sql)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(sql);
        }
        catch (Exception)
        {
            // Column already exists on an existing database.
        }
    }
}
