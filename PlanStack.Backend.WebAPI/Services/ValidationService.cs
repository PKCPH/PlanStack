using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Validation;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Services
{
    public class ValidationService
    {
        private readonly BlueprintBuildingStructureRepository _blueprintBuildingStructureRepository;
        private readonly BlueprintComponentRepository _blueprintComponentRepository;
        private readonly BlueprintStandardRepository _blueprintStandardRepository;
        private readonly RoomRepository _roomRepository;
        private readonly StandardRepository _standardRepository;
        private readonly StandardRuleSetRepository _standardRuleSetRepository;

        public ValidationService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository,
            BlueprintComponentRepository blueprintComponentRepository,
            BlueprintStandardRepository blueprintStandardRepository,
            RoomRepository roomRepository,
            StandardRepository standardRepository,
            StandardRuleSetRepository standardRuleSetRepository
        )
        {
            _blueprintBuildingStructureRepository = blueprintBuildingStructureRepository;
            _blueprintComponentRepository = blueprintComponentRepository;
            _blueprintStandardRepository = blueprintStandardRepository;
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
                        case RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_OVER_RATIO when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM:
                        case RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_UNDER_RATIO when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM:
                        case RuleSetDefinitionEnum.BY_BLUEPRINT_AREA_EXACT_RATIO when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM:
                            validationResource = await ValidateComponentPerRoomOverAreaAsync(blueprintId, standardRuleSet, validationResource);
                            if (!validationResource.IsValid)
                            {
                                validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}': Each room{GetRoomAreaConditionDescription(standardRuleSet)} must have at least one {standardRuleSet.RuleSet.ObjectTypeComparison}.");
                            }
                            break;

                        case RuleSetDefinitionEnum.BY_ROOM_AREA_SIZE_OVER_RATIO when standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM:
                            // TODO: Implement and call the correct validation method here
                            // validationResource = await ValidateRoomAreaSizeOverRatioAsync(blueprintId, standardRuleSet, validationResource);
                            // if (!validationResource.IsValid)
                            // {
                            //     validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}': Each room over {standardRuleSet.DefinitionValue} sqm must have at least one {standardRuleSet.RuleSet.ObjectTypeComparison}.");
                            // }
                            break;
                    }
                }

            }

            return validationResource;
        }
        #endregion

        #region ValidateComponentPerRoomOverAreaAsync
        public async Task<ValidationResource> ValidateComponentPerRoomOverAreaAsync(int blueprintId, StandardRuleSet standardRuleSet, ValidationResource validationResource)
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
            else if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_BLUEPRINT_AREA_EXACT_RATIO)
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

            validationResource.IsValid = true;

            return validationResource;
        }
        #endregion

        private string GetRoomAreaConditionDescription(StandardRuleSet standardRuleSet)
        {
            return standardRuleSet.RuleSet.Definition switch
            {
                RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_OVER_RATIO => $" over {standardRuleSet.DefinitionValue} sqm",
                RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_UNDER_RATIO => $" under {standardRuleSet.DefinitionValue} sqm",
                RuleSetDefinitionEnum.BY_BLUEPRINT_AREA_EXACT_RATIO => $" matching {standardRuleSet.DefinitionValue} sqm",
                _ => string.Empty
            };
        }
    }
}