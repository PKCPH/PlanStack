using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;

namespace PlanStack.Backend.WebAPI.Services
{
    public class BlueprintService
    {
        private readonly BlueprintBuildingStructureRepository _blueprintBuildingStructureRepository;
        private readonly BlueprintComponentRepository _blueprintComponentRepository;
        private readonly BuildingStructureRepository _buildingStructureRepository;
        private readonly BlueprintRepository _blueprintRepository;
        private readonly ComponentRepository _componentRepository;

        public BlueprintService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BlueprintComponentRepository blueprintComponentRepository,
            BuildingStructureRepository buildingStructureRepository,
            BlueprintRepository blueprintRepository,
            ComponentRepository componentRepository
        )
        {
            _blueprintBuildingStructureRepository = blueprintBuildingStructureRepository;
            _blueprintComponentRepository = blueprintComponentRepository;
            _buildingStructureRepository = buildingStructureRepository;
            _blueprintRepository = blueprintRepository;
            _componentRepository = componentRepository;
        }

        #region SaveBuildingStructuresToBlueprintAsync
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

        #region SaveComponentsToBlueprintAsync
        public async Task SaveComponentsToBlueprintAsync(Blueprint entity, List<BlueprintComponentSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintComponentRepository.GetAllByBlueprintIdAsync(entity.Id);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintComponentRepository.Remove(relation);
                }

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var component = await _componentRepository.GetAsync(saveResource.ComponentId.Value);
                    if (component != null)
                    {
                        var newRelation = new BlueprintComponent
                        {
                            BlueprintId = entity.Id,
                            ComponentId = component.Id,

                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _blueprintComponentRepository.Add(newRelation);

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