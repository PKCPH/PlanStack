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

                foreach (var standardRuleSet in standardRuleSets.Entities)
                {
                    if (standardRuleSet.RuleSet.Definition == RuleSetDefinitionEnum.BY_ROOM_AREA_OVER_RATIO && standardRuleSet.RuleSet.ObjectTypeDefinition == RuleSetObjectTypeEnum.ROOM)
                    {
                        validationResource = await ValidateComponentPerRoomOverAreaAsync(blueprintId, standardRuleSet, validationResource);
                        if (validationResource.IsValid == false)
                        {
                            validationResource.Errors.Add($"Validation failed for standard '{standard.Name}' - Rule Set '{standardRuleSet.RuleSet.Name}': Each room over {standardRuleSet.DefinitionValue} sqm must have at least one {standardRuleSet.RuleSet.ObjectTypeComparison}.");
                        }
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

            var roomsOverDefinitionValue = rooms.Entities
                .Where(x => x.SquareMeters > standardRuleSet.DefinitionValue)
                .ToList();

            if (roomsOverDefinitionValue.Count == 0)
            {
                validationResource.IsValid = false;
                validationResource.Errors.Add($"No rooms with square meters over {standardRuleSet.DefinitionValue} with the blueprint.");
                return validationResource;
            }

            foreach (var room in roomsOverDefinitionValue)
            {
                var fireSafetyEquipment = room.Components
                    .Where(c => c.Component != null && c.Component.Category.ToString() == standardRuleSet.RuleSet.ObjectTypeComparison.ToString())
                    .ToList();

                switch (standardRuleSet.RuleSet.Comparison)
                {
                    case RuleSetComparisonEnum.MINIMUM:
                        if (fireSafetyEquipment.Count < standardRuleSet.ComparisonValue)
                        {
                            validationResource.IsValid = false;
                            validationResource.Errors.Add($"Room '{room.Name}' does not meet the minimum required {standardRuleSet.RuleSet.ObjectTypeComparison} of {standardRuleSet.ComparisonValue}.");
                            return validationResource;
                        }
                        break;

                    case RuleSetComparisonEnum.MAXIMUM:
                        if (fireSafetyEquipment.Count > standardRuleSet.ComparisonValue)
                        {
                            validationResource.IsValid = false;
                            validationResource.Errors.Add($"Room '{room.Name}' exceeds the maximum allowed {standardRuleSet.RuleSet.ObjectTypeComparison} of {standardRuleSet.ComparisonValue}.");
                            return validationResource;
                        }
                        break;

                    case RuleSetComparisonEnum.EXACT:
                        if (fireSafetyEquipment.Count != standardRuleSet.ComparisonValue)
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
    }
}