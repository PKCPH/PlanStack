using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Component;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Services;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("components")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly ComponentRepository _componentRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;

        public ComponentController(
            ComponentRepository componentRepository,
            UnitOfWork unitOfWork,
            IMapper mapper,
            ImageService imageService
        )
        {
            _componentRepository = componentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<ComponentResource>> Create([FromForm] ComponentCreateResource createResource)
        {
            try
            {
                //Map entity
                var entity = _mapper.Map<ComponentCreateResource, Component>(createResource);

                var imagePath = _imageService.SaveImage(entity.Name, createResource.ImgFile);

                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
                entity.ImgPath = imagePath;

                // Add entity
                _componentRepository.Add(entity);

                // Save changes
                await _unitOfWork.SaveChangesAsync();

                // Map entity to resource
                var resource = _mapper.Map<Component, ComponentResource>(entity);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // [THE FIX] Return 200 OK, but with a success=false flag and the error message
                return Ok(new { success = false, message = $"Server Crash: {ex.Message}" });
            }
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<ComponentResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _componentRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            var resource = _mapper.Map<Component, ComponentResource>(entity);

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<ActionResult<BaseQueryResultResource<ComponentResource>>> GetAll([FromQuery] ComponentQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new ComponentQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                Category = filter.Category.HasValue
                           ? (ComponentCategoryEnum)filter.Category.Value
                           : null
            };

            // Get entities
            var queryResult = await _componentRepository.GetAllAsync(query);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<Component>, BaseQueryResultResource<ComponentResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromForm] ComponentUpdateResource updateResource)
        {
            var entity = await _componentRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            var imagePath = "";
            if (updateResource.ImgFile != null)
            {
                _imageService.DeleteImage(entity.ImgPath);
                imagePath = _imageService.SaveImage(entity.Name, updateResource.ImgFile);
                entity.ImgPath = imagePath;
            }

            entity.UpdatedAt = DateTime.Now;

            _mapper.Map<ComponentUpdateResource, Component>(updateResource, entity);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{entityId}")]
        public async Task<ActionResult> Delete(int entityId)
        {
            // Get entitty
            var entity = await _componentRepository.GetAsync(entityId);
            if (entity == null)
                return NotFound();

            //Delete entity
            try
            {
                _componentRepository.Remove(entity);

                _imageService.DeleteImage(entity.ImgPath);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
        #endregion
    }
}
