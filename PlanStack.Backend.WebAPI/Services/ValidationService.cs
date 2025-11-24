using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Validation;
using PlanStack.Shared.Enums;
using PlanStack.Backend.WebAPI.Services.Helpers;

namespace PlanStack.Backend.WebAPI.Services
{
    public class ValidationService
    {
        private readonly BlueprintComponentRepository _blueprintComponentRepository;
        private readonly BlueprintStandardRepository _blueprintStandardRepository;
        private readonly BlueprintRepository _blueprintRepository;
        private readonly RoomRepository _roomRepository;
        private readonly StandardRepository _standardRepository;
        private readonly StandardRuleSetRepository _standardRuleSetRepository;

        public ValidationService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BlueprintComponentRepository blueprintComponentRepository,
            BlueprintStandardRepository blueprintStandardRepository,
            BlueprintRepository blueprintRepository,
            RoomRepository roomRepository,
            StandardRepository standardRepository,
            StandardRuleSetRepository standardRuleSetRepository
        )
        {
            _blueprintComponentRepository = blueprintComponentRepository;
            _blueprintStandardRepository = blueprintStandardRepository;
            _blueprintRepository = blueprintRepository;
            _roomRepository = roomRepository;
            _standardRepository = standardRepository;
            _standardRuleSetRepository = standardRuleSetRepository;
        }

        #region ValidateBlueprintWithStandardsAsync
        public async Task<ValidationResource> ValidateBlueprintWithStandardsAsync(int blueprintId)
        {
            var validationResource = new ValidationResource();

            // Validate each rule set against the standard
            var blueprintStandards = await _blueprintStandardRepository.GetAllByBlueprintIdAsync(blueprintId);

            if (blueprintStandards == null || blueprintStandards.Entities.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("No standards associated with the blueprint.");
                return validationResource;
            }

            foreach (var blueprintStandard in blueprintStandards.Entities)
            {
                var standard = await _standardRepository.GetAsync(blueprintStandard.StandardId);

                var standardRuleSets = await _standardRuleSetRepository.GetAllByStandardIdAsync(standard.Id);

                if (standardRuleSets == null || standardRuleSets.Entities.Count == 0)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"No rule sets associated with the standard '{standard.Name}'.");
                    return validationResource;
                }

                foreach (var standardRuleSet in standardRuleSets.Entities)
                {
                    switch (standardRuleSet.RuleSet.Definition)
                    {
                        case RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_UNDER_RATIO:
                        case RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_EXACT_RATIO:
                            validationResource = await ValidateComponentPerRoomByAreaAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}': Each room{ValidationHelper.GetRoomAreaConditionDescription(standardRuleSet)} must have at least one {standardRuleSet.RuleSet.ObjectTypeComparison}.");
                            }
                            break;

                        case RuleSetDefinitionEnum.BY_ROOM_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_BATHROOM_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_LIVING_ROOM_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_KITCHEN_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_DINING_ROOM_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_OFFICE_AREA_SIZE_OVER_RATIO:
                        case RuleSetDefinitionEnum.BY_BEDROOM_AREA_SIZE_OVER_RATIO:
                            validationResource = await ValidateRoomTypeAreaSizeOverRatioAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                var roomType = ValidationHelper.GetRoomTypeFromDefinition(standardRuleSet.RuleSet.Definition);
                                validationResource.Errors.Add(
                                    $"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}': Every {roomType} must be over {standardRuleSet.DefinitionValue} sqm and have at least one {standardRuleSet.RuleSet.ObjectTypeComparison}."
                                );
                            }
                            break;

                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_PER_OCCUPANCY when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.OCCUPANCY:
                            validationResource = await ValidateComponentByOccupancyAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}'");
                            }
                            break;

                        case RuleSetDefinitionEnum.BY_ROOM_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM:
                        case RuleSetDefinitionEnum.BY_BATHROOM_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.BATHROOM:
                        case RuleSetDefinitionEnum.BY_LIVING_ROOM_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.LIVING_ROOM:
                        case RuleSetDefinitionEnum.BY_KITCHEN_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.KITCHEN:
                        case RuleSetDefinitionEnum.BY_DINING_ROOM_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.DINING_ROOM:
                        case RuleSetDefinitionEnum.BY_OFFICE_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.OFFICE:
                        case RuleSetDefinitionEnum.BY_BEDROOM_QUANTITY_IN_BLUEPRINT when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.BEDROOM:
                            validationResource = await ValidateRoomsInBlueprintAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}'");
                            }
                            break;

                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_ROOM:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BATHROOM:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_LIVING_ROOM:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_KITCHEN:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_DINING_ROOM:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_OFFICE:
                        case RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BEDROOM:
                            validationResource = await ValidateComponentByRoomTypeAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}'");
                            }
                            break;
                    }
                }

            }

            return validationResource;
        }
        #endregion

        #region ValidateComponentByRoomTypeAsync
        public async Task<ValidationResource> ValidateComponentByRoomTypeAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
        {
            var rooms = await _roomRepository.GetAllByBlueprintIdAsync(blueprintId);

            if (rooms.Entities.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("Blueprint does not have any rooms defined.");
                return validationResource;
            }

            var components = await _blueprintComponentRepository.GetAllByBlueprintIdAsync(blueprintId, true);

            var componentToValidate = components.Entities.Where(c => c.Component != null && c.Component.Category.ToString() == standardRuleSet.RuleSet.ObjectTypeDefinition.ToString()).ToList();

            if (componentToValidate.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add($"No components of type {standardRuleSet.RuleSet.ObjectTypeDefinition} found in the blueprint.");
                return validationResource;
            }

            var roomsWithType = standardRuleSet.RuleSet.Definition switch
            {
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BATHROOM => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.BATHROOM).ToList(),
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_LIVING_ROOM => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.LIVING_ROOM).ToList(),
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_KITCHEN => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.KITCHEN).ToList(),
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_DINING_ROOM => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.DINING_ROOM).ToList(),
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_OFFICE => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.OFFICE).ToList(),
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BEDROOM => rooms.Entities.Where(r => r.RoomType == RoomTypeEnum.BEDROOM).ToList(),
                _ => rooms.Entities
            };

            if (roomsWithType.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add($"No rooms of type {ValidationHelper.GetRoomTypeFromDefinition(standardRuleSet.RuleSet.Definition)} found in the blueprint.");
                return validationResource;
            }

            var requiredAmount = standardRuleSet.DefinitionValue;

            foreach (var room in roomsWithType)
            {
                if (room.Components == null || room.Components.Count == 0)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Room '{room.Name}' does not have any components.");
                    continue;
                }

                var componentsInRoom = room.Components
                    .Where(c => c.Component != null && c.Component.Category.ToString() == standardRuleSet.RuleSet.ObjectTypeDefinition.ToString())
                    .ToList();

                if (componentsInRoom.Count < requiredAmount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Room '{room.Name}' does not have enough components of type {standardRuleSet.RuleSet.ObjectTypeDefinition}.");
                }
            }

            if (validationResource.Errors.Count == 0)
                validationResource.IsValid = true;

            return validationResource;
        }
        #endregion

        #region ValidateRoomsInBlueprintAsync
        public async Task<ValidationResource> ValidateRoomsInBlueprintAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
        {
            var rooms = await _roomRepository.GetAllByBlueprintIdAsync(blueprintId);

            if (rooms.Entities.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("Blueprint does not have any rooms defined.");
                return validationResource;
            }

            var requiredRoomCount = standardRuleSet.DefinitionValue;

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM)
            {
                if (rooms.Entities.Count < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required room count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.BATHROOM)
            {
                var bathroomCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.BATHROOM);
                if (bathroomCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required bathroom count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.LIVING_ROOM)
            {
                var livingRoomCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.LIVING_ROOM);
                if (livingRoomCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required living room count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.KITCHEN)
            {
                var kitchenCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.KITCHEN);
                if (kitchenCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required kitchen count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.DINING_ROOM)
            {
                var diningRoomCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.DINING_ROOM);
                if (diningRoomCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required dining room count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.OFFICE)
            {
                var officeCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.OFFICE);
                if (officeCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required office count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.BEDROOM)
            {
                var bedroomCount = rooms.Entities.Count(r => r.RoomType == RoomTypeEnum.BEDROOM);
                if (bedroomCount < requiredRoomCount)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the required bedroom count of {requiredRoomCount}.");
                    return validationResource;
                }
            }

            if (validationResource.Errors.Count == 0)
                validationResource.IsValid = true;

            return validationResource;
        }
        #endregion

        #region ValidateComponentPerRoomByAreaAsync
        public async Task<ValidationResource> ValidateComponentPerRoomByAreaAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
        {
            var rooms = await _roomRepository.GetAllByBlueprintIdAsync(blueprintId, true);

            if (rooms == null)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("No rooms associated with the blueprint.");
                return validationResource;
            }

            var roomsByDefinitionValue = new List<Room>();
            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_UNDER_RATIO)
            {
                roomsByDefinitionValue = rooms.Entities
                    .Where(x => x.SquareMeters < standardRuleSet.DefinitionValue)
                    .ToList();
            }
            else if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_EXACT_RATIO)
            {
                roomsByDefinitionValue = rooms.Entities
                    .Where(x => x.SquareMeters == standardRuleSet.DefinitionValue)
                    .ToList();
            }
            else
            {
                roomsByDefinitionValue = rooms.Entities
                    .Where(x => x.SquareMeters > standardRuleSet.DefinitionValue)
                    .ToList();
            }

            if (roomsByDefinitionValue.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add($"No rooms with square meters over {standardRuleSet.DefinitionValue} with the blueprint.");
                return validationResource;
            }

            foreach (var room in roomsByDefinitionValue)
            {
                var componentToValidate = room.Components
                    .Where(c => c.Component != null && c.Component.Category.ToString() == standardRuleSet.RuleSet.ObjectTypeComparison.ToString())
                    .ToList();

                switch (standardRuleSet.RuleSet.Comparison)
                {
                    case RuleSetComparisonEnum.MINIMUM:
                        if (componentToValidate.Count < standardRuleSet.ComparisonValue)
                        {
                            validationResource.IsValid = false;
                            validationResource.Errors.Add($"Room '{room.Name}' does not meet the minimum required {standardRuleSet.RuleSet.ObjectTypeComparison} of {standardRuleSet.ComparisonValue}.");
                            return validationResource;
                        }
                        break;

                    case RuleSetComparisonEnum.MAXIMUM:
                        if (componentToValidate.Count > standardRuleSet.ComparisonValue)
                        {
                            validationResource.IsValid = false;
                            validationResource.Errors.Add($"Room '{room.Name}' exceeds the maximum allowed {standardRuleSet.RuleSet.ObjectTypeComparison} of {standardRuleSet.ComparisonValue}.");
                            return validationResource;
                        }
                        break;

                    case RuleSetComparisonEnum.EXACT:
                        if (componentToValidate.Count != standardRuleSet.ComparisonValue)
                        {
                            validationResource.IsValid = false;
                            validationResource.Errors.Add($"Room '{room.Name}' does not meet the exact required {standardRuleSet.RuleSet.ObjectTypeComparison} of {standardRuleSet.ComparisonValue}.");
                            return validationResource;
                        }
                        break;
                }
            }

            if (validationResource.Errors.Count == 0)
                validationResource.IsValid = true;

            return validationResource;
        }
        #endregion

        #region ValidateComponentByOccupancyAsync
        public async Task<ValidationResource> ValidateComponentByOccupancyAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
        {
            var blueprint = await _blueprintRepository.GetAsync(blueprintId);

            if (blueprint.MaxOccupancy <= 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("Blueprint does not have a valid maximum occupancy defined.");
                return validationResource;
            }

            var components = await _blueprintComponentRepository.GetAllByBlueprintIdAsync(blueprintId, true);

            var componentToValidate = components.Entities.Where(c => c.Component != null && c.Component.Category.ToString() == standardRuleSet.RuleSet.ObjectTypeComparison.ToString()).ToList();

            if (componentToValidate.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add($"No components of type {standardRuleSet.RuleSet.ObjectTypeComparison} found in the blueprint.");
                return validationResource;
            }

            var blueprintOccupancy = blueprint.MaxOccupancy;
            var occupancyRatio = standardRuleSet.DefinitionValue;

            var requiredComponents = (float)Math.Ceiling((float)blueprintOccupancy / (float)occupancyRatio);

            if (standardRuleSet.RuleSet.Comparison == RuleSetComparisonEnum.MINIMUM)
            {
                if (componentToValidate.Count < requiredComponents)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the minimum required {standardRuleSet.RuleSet.ObjectTypeComparison} of {requiredComponents} for occupancy of {blueprint.MaxOccupancy}.");
                    return validationResource;
                }
            }
            else if (standardRuleSet.RuleSet.Comparison == RuleSetComparisonEnum.MAXIMUM)
            {
                if (componentToValidate.Count > requiredComponents)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint exceeds the maximum allowed {standardRuleSet.RuleSet.ObjectTypeComparison} of {requiredComponents} for occupancy of {blueprint.MaxOccupancy}.");
                    return validationResource;
                }
            }
            else if (standardRuleSet.RuleSet.Comparison == RuleSetComparisonEnum.EXACT)
            {
                if (componentToValidate.Count != requiredComponents)
                {
                    validationResource.IsValid = false;
                    validationResource.Errors.Add($"Blueprint does not meet the exact required {standardRuleSet.RuleSet.ObjectTypeComparison} of {requiredComponents} for occupancy of {blueprint.MaxOccupancy}.");
                    return validationResource;
                }
            }

            if (validationResource.Errors.Count == 0)
                validationResource.IsValid = true;

            return validationResource;
        }
        #endregion

        #region ValidateRoomTypeAreaSizeOverRatioAsync
        public async Task<ValidationResource> ValidateRoomTypeAreaSizeOverRatioAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
        {
            var rooms = await _roomRepository.GetAllByBlueprintIdAsync(blueprintId, true);

            if (rooms == null)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add("No rooms associated with the blueprint.");
                return validationResource;
            }

            var nonMatchingRooms = new List<string>();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_BATHROOM_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.BATHROOM && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_LIVING_ROOM_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.LIVING_ROOM && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_KITCHEN_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.KITCHEN && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_DINING_ROOM_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.DINING_ROOM && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_OFFICE_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.OFFICE && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_BEDROOM_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.RoomType == RoomTypeEnum.BEDROOM && x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_ROOM_AREA_SIZE_OVER_RATIO)
                nonMatchingRooms = rooms.Entities.Where(x => x.SquareMeters <= standardRuleSet.DefinitionValue).Select(x => x.Name).ToList();

            if (nonMatchingRooms.Count == 0 && validationResource.Errors.Count == 0)
                validationResource.IsValid = true;
            else
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add(
                    $"The following rooms do not have more than {standardRuleSet.DefinitionValue} sqm: {string.Join(", ", nonMatchingRooms)}"
                );
            }

            return validationResource;
        }
        #endregion
    }
}