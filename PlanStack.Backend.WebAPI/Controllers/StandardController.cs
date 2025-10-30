using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Standard;

namespace Api.Controllers
{
    [Route("standards")]
    [ApiController]
    public class StandardController : ControllerBase
    {
        private readonly StandardRepository _standardRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StandardController(
            StandardRepository standardRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _standardRepository = standardRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<StandardResource>> Create([FromBody] StandardCreateResource createResource)
        {
            createResource.CreatedAt = DateTime.Now;
            createResource.UpdatedAt = DateTime.Now;

            //Map entity
            var entity = _mapper.Map<StandardCreateResource, Standard>(createResource);

            // Add entity
            _standardRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<Standard, StandardResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<StandardResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _standardRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<Standard, StandardResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<StandardResource>>> GetAll([FromQuery] StandardQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new StandardQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
            };

            // Get entities
            var queryResult = await _standardRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<Standard>, BaseQueryResultResource<StandardResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] StandardUpdateResource updateResource)
        {
            updateResource.UpdatedAt = DateTime.Now;

            var entity = await _standardRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            _mapper.Map<StandardUpdateResource, Standard>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entity
            var entity = await _standardRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _standardRepository.Remove(entity);

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
