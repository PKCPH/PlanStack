using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;

namespace Api.Controllers
{
    [Route("api/components")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly ComponentRepository _componentRepository;
        private readonly UnitOfWork _unitOfWork;

        public ComponentController(
            ComponentRepository componentRepository,
            UnitOfWork unitOfWork
            )
        {
            _componentRepository = componentRepository;
            _unitOfWork = unitOfWork;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<ComponentResource>> Create([FromBody] ComponentCreateResource createResource)
        {
            // Map entity
            //var entity = this.mapper.Map<ProductCreateResource, Product>(createResource);

            var entity = new Component
            {
                Model = createResource.Model,
                Price = createResource.Price,
                SquareMeters = createResource.SquareMeters,
                Name = createResource.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add entity
            _componentRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            //var resource = this.mapper.Map<Product, ProductResource>(entity);

            var resource = new ComponentResource
            {
                Model = entity.Model,
                Price = entity.Price,
                SquareMeters = entity.SquareMeters
            };

            return Ok(resource);
        }
        #endregion

        #region Get
        [HttpGet("{entityId}")]
        public async Task<ActionResult<ComponentResource>> Get(int entityId)
        {
            // Get entity
            var entity = await _componentRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            // Map entity to resource
            //var resource = this.mapper.Map<Product, ProductResource>(entity);

            var resource = new ComponentResource
            {
                Model = entity.Model,
                Price = entity.Price,
                SquareMeters = entity.SquareMeters
            };

            return Ok(resource);
        }
        #endregion

        #region GetAll
        [HttpGet, Authorize]
        public async Task<ActionResult<BaseQueryResultResource<ComponentResource>>> GetAll([FromQuery] ComponentQueryResource filter)
        {
            if (filter.PageSize == 0)
                filter.PageSize = -1;

            // Create query
            var query = new ComponentQuery()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
            };

            // Get entities
            var queryResult = await _componentRepository.GetAllAsync(query, true);

            // Map entities to resource
            //var resource = this.mapper.Map<BaseQueryResult<Product>, BaseQueryResultResource<ProductResource>>(queryResult);

            var resource = new BaseQueryResultResource<ComponentResource>
            {
                Entities = queryResult.Entities.Select(entity => new ComponentResource
                {
                    Model = entity.Model,
                    Price = entity.Price,
                    SquareMeters = entity.SquareMeters
                }),
                Count = queryResult.Count
            };

            return resource;
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] ComponentUpdateResource updateResource)
        {
            var entity = await _componentRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

            entity.Model = updateResource.Model;
            entity.Price = updateResource.Price;
            entity.SquareMeters = updateResource.SquareMeters;
            entity.UpdatedAt = updateResource.UpdatedAt;

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("delete/{entityId}")]
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
