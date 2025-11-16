using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Services;

namespace PlanStack.Backend.WebAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("blueprints")]
    [ApiController]
    public class BlueprintController : ControllerBase
    {
        private readonly BlueprintRepository _blueprintRepository;
        private readonly BlueprintService _blueprintService;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlueprintController(
            BlueprintRepository blueprintRepository,
            BlueprintService blueprintService,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _blueprintRepository = blueprintRepository;
            _blueprintService = blueprintService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<BlueprintResource>> Create([FromBody] BlueprintCreateResource createResource)
        {
            //Map entity
            var entity = _mapper.Map<BlueprintCreateResource, Blueprint>(createResource);

            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // Add entity
            _blueprintRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<Blueprint, BlueprintResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<BlueprintResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _blueprintRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<Blueprint, BlueprintResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<BlueprintResource>>> GetAll([FromQuery] BlueprintQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new BlueprintQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                ProjectId = filter.ProjectId
            };

            // Get entities
            var queryResult = await _blueprintRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<Blueprint>, BaseQueryResultResource<BlueprintResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] BlueprintUpdateResource updateResource)
        {
            var entity = await _blueprintRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            entity.UpdatedAt = DateTime.Now;

            _mapper.Map<BlueprintUpdateResource, Blueprint>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entity
            var entity = await _blueprintRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _blueprintRepository.Remove(entity);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
        #endregion

        #region Save
        [HttpPut("save/{entityId}")]
        public async Task<ActionResult> Save(int entityId, [FromBody] BlueprintSaveResource saveResource)
        {
            var entity = await _blueprintRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            try
            {
                    await _blueprintService.SaveRoomsToBlueprintAsync(entity.Id, saveResource.Rooms);
                    await _blueprintService.SaveBuildingStructuresToBlueprintAsync(entity.Id, saveResource.BuildingStructures);
                    await _blueprintService.SaveComponentsToBlueprintAsync(entity.Id, saveResource.Components);
                    await _blueprintService.SaveStandardsToBlueprintAsync(entity.Id, saveResource.Standards);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = $"Server Crash: {ex.Message}" });
            }

            _mapper.Map<BlueprintSaveResource, Blueprint>(saveResource, entity);

            entity.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region ValidateBlueprint
        [HttpGet("validate/{entityId}")]
        public async Task<ActionResult<BlueprintResource>> ValidateBlueprint(int entityId)
        {
            //Get entity
            var entity = await _blueprintRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            // Validate blueprint - With ValidateService
            var validateSuccess = await _blueprintService.ValidateBlueprintWithStandardsAsync(entityId);

            if (validateSuccess == true)
            {
                entity.IsValidated = true;
                entity.UpdatedAt = DateTime.Now;
            }
            else
            {
                entity.IsValidated = false;
            }
            
            await _unitOfWork.SaveChangesAsync();

            return Ok("");
        }
        #endregion
    }
}
