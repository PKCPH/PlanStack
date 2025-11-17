using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;
using PlanStack.Backend.WebAPI.Controllers.Resources.Room;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Validation;
using PlanStack.Shared.Enums;

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
        private readonly StandardRuleSetRepository _standardRuleSetRepository;

        public BlueprintService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BlueprintComponentRepository blueprintComponentRepository,
            BlueprintStandardRepository blueprintStandardRepository,
            BuildingStructureRepository buildingStructureRepository,
            BlueprintRepository blueprintRepository,
            ComponentRepository componentRepository,
            RoomRepository roomRepository,
            StandardRepository standardRepository,
            StandardRuleSetRepository standardRuleSetRepository
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
            _standardRuleSetRepository = standardRuleSetRepository;
        }

        #region SaveBuildingStructuresToBlueprintAsync
        public async Task SaveBuildingStructuresToBlueprintAsync(int blueprintId, List<BlueprintBuildingStructureSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintBuildingStructureRepository.GetAllByBlueprintIdAsync(blueprintId);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintBuildingStructureRepository.Remove(relation);
                }

                // Null check
                if (saveResources == null)
                    return;

                if (saveResources.Count == 0)
                    return;

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var buildingStructure = await _buildingStructureRepository.GetAsync(saveResource.BuildingStructureId.Value);
                    if (buildingStructure != null)
                    {
                        var newRelation = new BlueprintBuildingStructure
                        {
                            BlueprintId = blueprintId,
                            BuildingStructureId = buildingStructure.Id,
                            Height = saveResource.Height,
                            Width = saveResource.Width,
                            StartingPositionX = saveResource.StartingPositionX,
                            StartingPositionY = saveResource.StartingPositionY,

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
                throw new InvalidOperationException($"Error saving blueprint components for blueprint '{blueprintId}'.", ex);
            }
        }
        #endregion

        #region SaveComponentsToBlueprintAsync
        public async Task SaveComponentsToBlueprintAsync(int blueprintId, List<BlueprintComponentSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintComponentRepository.GetAllByBlueprintIdAsync(blueprintId);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintComponentRepository.Remove(relation);
                }

                // Null check
                if (saveResources == null)
                    return;

                if (saveResources.Count == 0)
                    return;

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var component = await _componentRepository.GetAsync(saveResource.ComponentId.Value);
                    if (component != null)
                    {
                        var newRelation = new BlueprintComponent
                        {
                            BlueprintId = blueprintId,
                            ComponentId = component.Id,

                            IsHorizontal = saveResource.IsHorizontal,
                            StartingPositionX = saveResource.StartingPositionX,
                            StartingPositionY = saveResource.StartingPositionY,
                            RoomId = Guid.TryParse(saveResource?.RoomId, out var guid) ? guid : null,

                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _blueprintComponentRepository.Add(newRelation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving blueprint components for blueprint '{blueprintId}'.", ex);
            }
        }
        #endregion

        #region SaveRoomsToBlueprintAsync
        public async Task SaveRoomsToBlueprintAsync(int blueprintId, List<RoomSaveResource> saveResources)
        {
            try
            {
                // Remove existing rooms
                var existingRoom = await _roomRepository.GetAllByBlueprintIdAsync(blueprintId);
                foreach (var room in existingRoom.Entities)
                {
                    _roomRepository.Remove(room);
                }

                // Null check
                if (saveResources == null)
                    return;

                if (saveResources.Count == 0)
                    return;

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var newRoom = new Room
                    {
                        Id = Guid.Parse(saveResource.Id),
                        BlueprintId = blueprintId,
                        RoomType = saveResource.RoomType,
                        Name = saveResource.Name,
                        SquareMeters = saveResource.SquareMeters,

                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _roomRepository.Add(newRoom);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving rooms for blueprint '{blueprintId}'.", ex);
            }
        }
        #endregion

        #region SaveStandardsToBlueprintAsync
        public async Task SaveStandardsToBlueprintAsync(int blueprintId, List<BlueprintStandardSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintStandardRepository.GetAllByBlueprintIdAsync(blueprintId);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintStandardRepository.Remove(relation);
                }

                // Null check
                if (saveResources == null)
                    return;

                if (saveResources.Count == 0)
                    return;

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var standard = await _standardRepository.GetAsync(saveResource.StandardId);
                    if (standard != null)
                    {
                        var newRelation = new BlueprintStandard
                        {
                            BlueprintId = blueprintId,
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
                throw new InvalidOperationException($"Error saving blueprint standards for blueprint '{blueprintId}'.", ex);
            }
        }
        #endregion

        #region CreateStandardsToBlueprintAsync
        public async Task CreateStandardsToBlueprintAsync(int blueprintId, List<BlueprintStandardCreateResource> createResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _blueprintStandardRepository.GetAllByBlueprintIdAsync(blueprintId);
                foreach (var relation in existingRelations.Entities)
                {
                    _blueprintStandardRepository.Remove(relation);
                }

                // Null check
                if (createResources == null)
                    return;

                if (createResources.Count == 0)
                    return;

                // Add new relations
                foreach (var createResource in createResources)
                {
                    var standard = await _standardRepository.GetAsync(createResource.StandardId);
                    if (standard != null)
                    {
                        var newRelation = new BlueprintStandard
                        {
                            BlueprintId = blueprintId,
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
                throw new InvalidOperationException($"Error saving blueprint standards for blueprint '{blueprintId}'.", ex);
            }
        }
        #endregion

    }
}