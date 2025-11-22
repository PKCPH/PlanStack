using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Shared.Enums;

namespace PlanStack.UnitTests
{
    [TestClass]
    public class RepositoryTest
    {
        protected DatabaseContext context;
        public DbContextOptions<DatabaseContext> options;

        public RepositoryTest()
        {
            options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "dummyDatabase")
                .Options;
            context = new DatabaseContext(options);
            context.Database.EnsureDeleted();
            context.BuildingStructures.Add(new BuildingStructure
            {
                Id = 1,
                Name = "Wooden Wall",
                Description = "A sturdy wooden wall.",
                Category = BuildingStructureCategoryEnum.WALL,
                Material = "Wood",
                Price = 200
            });
            context.SaveChanges();
        }

        [TestMethod]
        public async Task IsGetNotNull()
        {
            // Arrange
            BuildingStructureRepository repo = new BuildingStructureRepository(context);

            // Act
            var result = await repo.GetAsync(1);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod] 
        public async Task IsGetAllNotNull() 
        {
            // Arrange
            var query = new BuildingStructureQuery()
            {
                PageSize = -1
            };

            BuildingStructureRepository repo = new BuildingStructureRepository(context);

            // Act
            var result = await repo.GetAllAsync(query);

            // Assert
            Assert.IsNotNull(result); 
        }

        [TestMethod] 
        public async Task DeleteReturnTrue()
        {
            // Arrange
            var entityToRemove = await context.BuildingStructures.FirstOrDefaultAsync(h => h.Id == 1);
            BuildingStructureRepository repo = new BuildingStructureRepository(context);

            // Act
            repo.Remove(entityToRemove);
            await context.SaveChangesAsync();
            var entityAfterRemoval = await context.BuildingStructures.FirstOrDefaultAsync(h => h.Id == 1);

            // Assert
            Assert.IsNull(entityAfterRemoval); 
        }

        [TestMethod]
        public async Task CreateReturnNotNull()
        {
            // Arrange
            var entityToAdd = new BuildingStructure
            {
                Id = 2,
                Name = "Concrete Wall",
                Description = "A sturdy concrete wall.",
                Category = BuildingStructureCategoryEnum.WALL,
                Material = "Concrete",
                Price = 200
            };

            // Act
            BuildingStructureRepository repo = new BuildingStructureRepository(context);
            repo.Add(entityToAdd);
            await context.SaveChangesAsync();
            var entityAfterCreation = await context.BuildingStructures.FirstOrDefaultAsync(h => h.Id == entityToAdd.Id);

            // Assert
            Assert.IsNotNull(entityAfterCreation); 
        }
    }
}