﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Component;

namespace Api.Controllers
{
    [Route("components")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly ComponentRepository _componentRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComponentController(
            ComponentRepository componentRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _componentRepository = componentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<ComponentResource>> Create([FromBody] ComponentCreateResource createResource)
        {
            createResource.CreatedAt = DateTime.Now;
            createResource.UpdatedAt = DateTime.Now;

            //Map entity
            var entity = _mapper.Map<ComponentCreateResource, Component>(createResource);

            // Add entity
            _componentRepository.Add(entity);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Map entity to resource
            var resource = _mapper.Map<Component, ComponentResource>(entity);

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
            };

            // Get entities
            var queryResult = await _componentRepository.GetAllAsync(query, true);

            // Map entities to resource
            var resource = _mapper.Map<BaseQueryResult<Component>, BaseQueryResultResource<ComponentResource>>(queryResult);

            return Ok(resource);
        }
        #endregion

        #region Update
        [HttpPut("{entityId}")]
        public async Task<ActionResult> Update(int entityId, [FromBody] ComponentUpdateResource updateResource)
        {
            updateResource.UpdatedAt = DateTime.Now;

            var entity = await _componentRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

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
