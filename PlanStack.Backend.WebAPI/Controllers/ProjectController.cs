using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Project;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectRepository _projectRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectController(
            ProjectRepository projectRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<ProjectResource>> Create([FromBody] ProjectCreateResource createResource)
        {
            //Map entity
            var entity = _mapper.Map<ProjectCreateResource, Project>(createResource);

            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // Add entity
            _projectRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<Project, ProjectResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<ProjectResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _projectRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<Project, ProjectResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<ProjectResource>>> GetAll([FromQuery] ProjectQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new ProjectQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
            };

            // Get entities
            var queryResult = await _projectRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<Project>, BaseQueryResultResource<ProjectResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] ProjectUpdateResource updateResource)
        {
            var entity = await _projectRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            entity.UpdatedAt = DateTime.Now;

            _mapper.Map<ProjectUpdateResource, Project>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entitty
            var entity = await _projectRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _projectRepository.Remove(entity);

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
