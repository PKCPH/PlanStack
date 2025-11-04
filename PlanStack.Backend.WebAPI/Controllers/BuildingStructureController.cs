using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("buildingstructures")]
    [ApiController]
    public class BuildingStructureController : ControllerBase
    {
        private readonly BuildingStructureRepository _buildingStructureRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BuildingStructureController(
            BuildingStructureRepository buildingStructureRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _buildingStructureRepository = buildingStructureRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<BuildingStructureResource>> Create([FromBody] BuildingStructureCreateResource createResource)
        {
            createResource.CreatedAt = DateTime.Now;
            createResource.UpdatedAt = DateTime.Now;

            //Map entity
            var entity = _mapper.Map<BuildingStructureCreateResource, BuildingStructure>(createResource);

            // Add entity
            _buildingStructureRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<BuildingStructure, BuildingStructureResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<BuildingStructureResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _buildingStructureRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<BuildingStructure, BuildingStructureResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<BuildingStructureResource>>> GetAll([FromQuery] BuildingStructureQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new BuildingStructureQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
            };

            // Get entities
            var queryResult = await _buildingStructureRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<BuildingStructure>, BaseQueryResultResource<BuildingStructureResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] BuildingStructureUpdateResource updateResource)
        {
            updateResource.UpdatedAt = DateTime.Now;

            var entity = await _buildingStructureRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            _mapper.Map<BuildingStructureUpdateResource, BuildingStructure>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entity
            var entity = await _buildingStructureRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _buildingStructureRepository.Remove(entity);

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
