using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;

namespace PlanStack.Backend.WebAPI.Services
{
    public class StandardService
    {
        private readonly StandardRuleSetRepository _standardRuleSetRepository;
        private readonly RuleSetRepository _ruleSetRepository;

        public StandardService(
            StandardRuleSetRepository standardRuleSetRepository,
            RuleSetRepository ruleSetRepository
        )
        {
            _standardRuleSetRepository = standardRuleSetRepository;
            _ruleSetRepository = ruleSetRepository;
        }

        #region SaveRuleSetsToStandardAsync
        public async Task SaveRuleSetsToStandardAsync(int standardId, List<StandardRuleSetSaveResource> saveResources)
        {
            try
            {
                // Remove existing relations
                var existingRelations = await _standardRuleSetRepository.GetAllByStandardIdAsync(standardId);
                foreach (var relation in existingRelations.Entities)
                {
                    _standardRuleSetRepository.Remove(relation);
                }

                // Add new relations
                foreach (var saveResource in saveResources)
                {
                    var ruleSet = await _ruleSetRepository.GetAsync(saveResource.RuleSetId);
                    if (ruleSet != null)
                    {
                        var newRelation = new StandardRuleSet
                        {
                            UserDefinedName = saveResource.UserDefinedName,
                            StandardId = standardId,
                            RuleSetId = ruleSet.Id,

                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _standardRuleSetRepository.Add(newRelation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving rule sets for standard '{standardId}'.", ex);
            }
        }
        #endregion
    }
}