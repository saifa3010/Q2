// Infrastructure/Persistence/Seeders/DatabaseSeeder.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
                return; // Already seeded

            var user = AppUser.Create(
                keycloakUserId: "seed-user-001",
                fullName: "Saif Test",
                email: "saif@test.com",
                phoneNumber: "0791234567");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded default user: {Email}", user.Email);
        }
    }
}