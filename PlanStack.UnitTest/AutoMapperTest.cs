using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;
using PlanStack.Shared.Enums;
using PlanStack.UnitTest.Managers;

namespace PlanStack.UnitTests
{
    [TestClass]
    public class AutoMapperTest
    {
        private static IMapper _mapper = AutoMapperManager.GetMapper();

        private static List<BuildingStructure> _entities;

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            var connection = "Server=(localdb)\\mssqllocaldb;Database=PlanstackDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(connection);

            using var dbContext = new DatabaseContext(contextOptions.Options);

            var repository = new BuildingStructureRepository(dbContext);

            var queryResult = await repository.GetAllAsync(new());

            _entities = queryResult.Entities;
        }

        [TestMethod]
        public void DataModelToResourceModel()
        {
            foreach (var entity in _entities)
            {
                var autoMapperResource = _mapper.Map<BuildingStructure, BuildingStructureResource>(entity);

                autoMapperResource.Should().BeEquivalentTo(entity);
            }
        }

        [TestMethod]
        public void ResourceModelToDataModel()
        {
            var resourceEntity = new BuildingStructureResource
            {
                Id = 1,
                Name = "Wooden Wall",
                Description = "A sturdy wooden wall.",
                Category = BuildingStructureCategoryEnum.WALL,
                Material = "Wood",
                Price = 200
            };

            var autoMapperDataModel = _mapper.Map<BuildingStructureResource, BuildingStructure>(resourceEntity);

            autoMapperDataModel.Should().BeEquivalentTo(resourceEntity);
        }

        public void lol()
        {
            var resourceModel = new BuildingStructureResource
            {
                Id = 1,
                Name = "Wooden Wall",
                Description = "A sturdy wooden wall.",
                Category = BuildingStructureCategoryEnum.WALL,
                Material = "Wood",
                Price = 200
            };

            var dataModel = new BuildingStructure
            {
                Id = resourceModel.Id,
                Name = resourceModel.Name,
                Description = resourceModel.Description,
                Category = resourceModel.Category,
                Material = resourceModel.Material,
                Price = resourceModel.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}