using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("rulesets")]
    [ApiController]
    public class RuleSetController : ControllerBase
    {
        private readonly RuleSetRepository _ruleSetRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RuleSetController(
            RuleSetRepository ruleSetRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _ruleSetRepository = ruleSetRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<RuleSetResource>> Create([FromBody] RuleSetCreateResource createResource)
        {
            createResource.CreatedAt = DateTime.Now;
            createResource.UpdatedAt = DateTime.Now;

            //Map entity
            var entity = _mapper.Map<RuleSetCreateResource, RuleSet>(createResource);

            // Add entity
            _ruleSetRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<RuleSet, RuleSetResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<RuleSetResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _ruleSetRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<RuleSet, RuleSetResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<RuleSetResource>>> GetAll([FromQuery] RuleSetQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new RuleSetQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
            };

            // Get entities
            var queryResult = await _ruleSetRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<RuleSet>, BaseQueryResultResource<RuleSetResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] RuleSetUpdateResource updateResource)
        {
            updateResource.UpdatedAt = DateTime.Now;

            var entity = await _ruleSetRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            _mapper.Map<RuleSetUpdateResource, RuleSet>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entity
            var entity = await _ruleSetRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _ruleSetRepository.Remove(entity);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
        #endregion
    }
}
