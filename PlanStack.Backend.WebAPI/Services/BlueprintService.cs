using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;
using PlanStack.Backend.WebAPI.Controllers.Resources.Room;

namespace PlanStack.Backend.WebAPI.Services
{
    public class BlueprintService
    {
        private readonly BlueprintBuildingStructureRepository _blueprintBuildingStructureRepository;
        private readonly BlueprintComponentRepository _blueprintComponentRepository;
        private readonly BlueprintStandardRepository _blueprintStandardRepository;
        private readonly BuildingStructureRepository _buildingStructureRepository;
        private readonly BlueprintRepository _blueprintRepository;
        private readonly ComponentRepository _componentRepository;
        private readonly RoomRepository _roomRepository;
        private readonly StandardRepository _standardRepository;

        public BlueprintService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BlueprintComponentRepository blueprintComponentRepository,
            BlueprintStandardRepository blueprintStandardRepository,
            BuildingStructureRepository buildingStructureRepository,
            BlueprintRepository blueprintRepository,
            ComponentRepository componentRepository,
            RoomRepository roomRepository,
            StandardRepository standardRepository
        )
        {
            _blueprintBuildingStructureRepository = blueprintBuildingStructureRepository;
            _blueprintComponentRepository = blueprintComponentRepository;
            _blueprintStandardRepository = blueprintStandardRepository;
            _buildingStructureRepository = buildingStructureRepository;
            _blueprintRepository = blueprintRepository;
            _componentRepository = componentRepository;
            _roomRepository = roomRepository;
            _standardRepository = standardRepository;
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving blueprint components for blueprint '{entity.Id}'.", ex);
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving blueprint components for blueprint '{entity.Id}'.", ex);
            }
        }
        #endregion

        #region SaveRoomsToBlueprintAsync
        public async Task SaveRoomsToBlueprintAsync(Blueprint entity, List<RoomSaveResource> saveResources)
        {
            try
            {
                // Remove existing rooms
                var existingRoom = await _roomRepository.GetAllByBlueprintIdAsync(entity.Id);
                foreach (var room in existingRoom.Entities)
                {
                    _roomRepository.Remove(room);
                }

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var newRoom = new Room
                    {
                        BlueprintId = entity.Id,
                        RoomType = saveResource.RoomType,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _roomRepository.Add(newRoom);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving rooms for blueprint '{entity.Id}'.", ex);
            }
        }
        #endregion

        #region SaveStandardsToBlueprintAsync
        public async Task SaveStandardsToBlueprintAsync(Blueprint entity, List<BlueprintStandardSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintStandardRepository.GetAllByBlueprintIdAsync(entity.Id);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintStandardRepository.Remove(relation);
                }

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var standard = await _standardRepository.GetAsync(saveResource.StandardId);
                    if (standard != null)
                    {
                        var newRelation = new BlueprintStandard
                        {
                            BlueprintId = entity.Id,
                            StandardId = standard.Id,

                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _blueprintStandardRepository.Add(newRelation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving blueprint standards for blueprint '{entity.Id}'.", ex);
            }
        }
        #endregion
    }
}