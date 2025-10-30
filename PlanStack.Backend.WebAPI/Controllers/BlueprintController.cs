﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint;

namespace Api.Controllers
{
    [Route("blueprints")]
    [ApiController]
    public class BlueprintController : ControllerBase
    {
        private readonly BlueprintRepository _blueprintRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlueprintController(
            BlueprintRepository blueprintRepository,
            UnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _blueprintRepository = blueprintRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create
        [HttpPost()]
        public async Task<ActionResult<BlueprintResource>> Create([FromBody] BlueprintCreateResource createResource)
        {
            createResource.CreatedAt = DateTime.Now;
            createResource.UpdatedAt = DateTime.Now;

            //Map entity
            var entity = _mapper.Map<BlueprintCreateResource, Blueprint>(createResource);

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
            updateResource.UpdatedAt = DateTime.Now;

            var entity = await _blueprintRepository.GetAsync(entityId, true);
            if (entity == null)
                return NotFound();

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
    }
}
