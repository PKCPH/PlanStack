using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Backend.WebAPI.Handlers;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;

        public UsersController(
            UserManager<User> userManager,
            IMapper mapper,
            UserRepository userRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        //[HttpGet("{userId}")]
        //public async Task<ActionResult<UserResource>> Get(string userId)
        //{
        //    var user = await _userRepository.GetUserWithStats(userId);
        //    if (user == null)
        //        return NotFound("User was not found");

        //    var resource = _mapper.Map<UserResource>(user);

        //    resource.Stats = resource.Stats.OrderByDescending(s => s.Version).ToList();

        //    return Ok(resource);
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAll()
        {
            var users = await _userManager.Users
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{entityId}")]
        public async Task<IActionResult> Update(string entityId, [FromBody] UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(entityId);
            if (user == null)
                return NotFound("User was not found");

            // Could easily implement username change too here!
            // user.Username == request.Username

            if (!string.IsNullOrWhiteSpace(request.CurrentPassword) && !string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!passwordChangeResult.Succeeded)
                    return BadRequest(new { Errors = passwordChangeResult.Errors.Select(e => e.Description) });
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });

            var updatedResource = _mapper.Map<UserResource>(user);
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);

            if (userToDelete == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(userToDelete);

            if (result.Succeeded)
                return Ok();
            else
                return BadRequest("Error deleting user.");
        }
    }
}
