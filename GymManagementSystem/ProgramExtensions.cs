using GymManagementSystem.DAL.DataSeeding;
using GymManagementSystem.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem
{
    public static class ProgramExtensions
    {
        public static async Task MigrationAndSeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Apply {pendingMigrations.Count()} Pending Migrations");
                await dbContext.Database.MigrateAsync();
            }

            var SeedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, SeedFolderPath, logger);
        }

    }
}
