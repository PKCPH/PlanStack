using PlanStack.Backend.Database.DataModels;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Services.Helpers
{
    public static class ValidationHelper
    {
        public static string GetRoomAreaConditionDescription(StandardRuleSet standardRuleSet)
        {
            return standardRuleSet.RuleSet.Definition switch
            {
                RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_OVER_RATIO => $" over {standardRuleSet.DefinitionValue} sqm",
                RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_UNDER_RATIO => $" under {standardRuleSet.DefinitionValue} sqm",
                RuleSetDefinitionEnum.BY_COMPONENT_IN_ROOM_AREA_EXACT_RATIO => $" matching {standardRuleSet.DefinitionValue} sqm",
                _ => string.Empty
            };
        }

        public static string GetRoomTypeFromDefinition(RuleSetDefinitionEnum definition)
        {
            return definition switch
            {
                RuleSetDefinitionEnum.BY_BATHROOM_AREA_SIZE_OVER_RATIO => "bathroom",
                RuleSetDefinitionEnum.BY_LIVING_ROOM_AREA_SIZE_OVER_RATIO => "living room",
                RuleSetDefinitionEnum.BY_KITCHEN_AREA_SIZE_OVER_RATIO => "kitchen",
                RuleSetDefinitionEnum.BY_DINING_ROOM_AREA_SIZE_OVER_RATIO => "dining room",
                RuleSetDefinitionEnum.BY_OFFICE_AREA_SIZE_OVER_RATIO => "office",
                RuleSetDefinitionEnum.BY_BEDROOM_AREA_SIZE_OVER_RATIO => "bedroom",
                RuleSetDefinitionEnum.BY_ROOM_AREA_SIZE_OVER_RATIO => "room",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BATHROOM => "BATHROOM",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_LIVING_ROOM => "LIVING_ROOM",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_KITCHEN => "KITCHEN",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_DINING_ROOM => "DINING_ROOM",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_OFFICE => "OFFICE",
                RuleSetDefinitionEnum.BY_COMPONENT_QUANTITY_IN_BEDROOM => "BEDROOM",
                _ => "ROOM"
            };
        }
    }
}