using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint;

namespace PlanStack.Backend.WebAPI.Services
{
    public class BlueprintService
    {
        private readonly BlueprintBuildingStructureRepository _blueprintBuildingStructureRepository;
        private readonly BuildingStructureRepository _buildingStructureRepository;
        private readonly BlueprintRepository _blueprintRepository;

        public BlueprintService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BuildingStructureRepository buildingStructureRepository,
            BlueprintRepository blueprintRepository
        )
        {
            _blueprintBuildingStructureRepository = blueprintBuildingStructureRepository;
            _buildingStructureRepository = buildingStructureRepository;
            _blueprintRepository = blueprintRepository;
        }

        #region SaveBuildingStructuresToBlueprint
        public async Task SaveBuildingStructuresToBlueprintAsync(Blueprint entity, List<BlueprintBuildingStructureSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintBuildingStructureRepository.GetAllByBlueprintIdAsync(entity.Id);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintBuildingStructureRepository.Remove(relation);
                }

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var buildingStructure = await _buildingStructureRepository.GetAsync(saveResource.BuildingStructureId.Value);
                    if (buildingStructure != null)
                    {
                        var newRelation = new BlueprintBuildingStructure
                        {
                            BlueprintId = entity.Id,
                            BuildingStructureId = buildingStructure.Id,
                            Height = saveResource.Height,
                            Width = saveResource.Width,
                            
                            TotalPrice = 0,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _blueprintBuildingStructureRepository.Add(newRelation);

                        //saveResource.Positions?.ForEach(position =>
                        //{
                        //    var bsPosition = new BSBlueprintPosition
                        //    {
                        //        BlueprintBuildingStructureId = newRelation.Id,
                        //        X = position.X,
                        //        Y = position.Y,
                        //        CreatedAt = DateTime.Now,
                        //        UpdatedAt = DateTime.Now
                        //    };
                        //    newRelation.Positions.Add(bsPosition);
                        //});
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}