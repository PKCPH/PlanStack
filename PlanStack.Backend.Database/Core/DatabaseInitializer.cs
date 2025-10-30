using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlanStack.Backend.Database.DataModels;

namespace PlanStack.Backend.Database.Core
{
    public class DatabaseInitializer
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var adminEmail = "admin@planstack.dk";
            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            var existingClaim = await dbContext.UserClaims.FindAsync(1);
            if (existingUser == null)
            {
                var adminUser = new User
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "MySuperSecurePassword123!");
                await dbContext.SaveChangesAsync();

                var newUser = await userManager.FindByEmailAsync(adminEmail);
                if (existingClaim == null && result.Succeeded && newUser != null)
                    await userManager.AddToRoleAsync(newUser, "Admin");

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
