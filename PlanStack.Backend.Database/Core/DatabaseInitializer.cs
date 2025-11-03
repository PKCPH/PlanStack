using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;

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

        public static async Task SeedComponentsAsync(IServiceProvider serviceProvider)
        {
            //using var scope = serviceProvider.CreateScope();
            //var componentRepository = scope.ServiceProvider.GetRequiredService<ComponentRepository>();
            //var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            //var existingComponents = await componentRepository.GetAllAsync(new ComponentQuery { PageSize = -1 });

            //if (existingComponents.Count == 0)
            //{
            //    var component1 = new Component
            //    {
            //        Name = $"Component",
            //        Description = $"Description for Component",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var component2 = new Component
            //    {
            //        Name = $"Another Component",
            //        Description = $"Description for Another Component",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var component3 = new Component
            //    {
            //        Name = $"Another Component",
            //        Description = $"Description for Another Component",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var component4 = new Component
            //    {
            //        Name = $"Another Component",
            //        Description = $"Description for Another Component",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var component5 = new Component
            //    {
            //        Name = $"Another Component",
            //        Description = $"Description for Another Component",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };

            //    var components = new List<Component> { component1, component2, component3, component4, component5 };

            //    foreach (var component in components)
            //        componentRepository.Add(component);

            //    await unitOfWork.SaveChangesAsync();
            //}
        }

        public static async Task SeedBuildingStructuresAsync(IServiceProvider serviceProvider)
        {
            //using var scope = serviceProvider.CreateScope();
            //var buildingStructureRepository = scope.ServiceProvider.GetRequiredService<BuildingStructureRepository>();
            //var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            //var existingBuildingStructures = await buildingStructureRepository.GetAllAsync(new BuildingStructureQuery { PageSize = -1 });

            //if (existingBuildingStructures.Count == 0)
            //{
            //    var buildingStructure1 = new BuildingStructure
            //    {
            //        Name = $"Building Structure",
            //        Description = $"Description for Building Structure",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var buildingStructure2 = new BuildingStructure
            //    {
            //        Name = $"Another Building Structure",
            //        Description = $"Description for Another Building Structure",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var buildingStructure3 = new BuildingStructure
            //    {
            //        Name = $"Another Building Structure",
            //        Description = $"Description for Another Building Structure",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var buildingStructure4 = new BuildingStructure
            //    {
            //        Name = $"Another Building Structure",
            //        Description = $"Description for Another Building Structure",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };
            //    var buildingStructure5 = new BuildingStructure
            //    {
            //        Name = $"Another Building Structure",
            //        Description = $"Description for Another Building Structure",
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    };

            //    var buildingStructures = new List<BuildingStructure> { buildingStructure1, buildingStructure2, buildingStructure3, buildingStructure4, buildingStructure5 };

            //    foreach (var buildingStructure in buildingStructures)
            //        buildingStructureRepository.Add(buildingStructure);

            //    await unitOfWork.SaveChangesAsync();
            //}
        }
    }
}
