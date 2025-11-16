using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Backend.WebAPI.Extensions;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly UnitOfWork _unitOfWork;

        public UsersController(
            UserManager<User> userManager,
            IMapper mapper,
            UserRepository userRepository,
            UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResource>> Get(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
                return NotFound(new { Errors = "User was not found." });

            var resource = _mapper.Map<UserResource>(user);

            return Ok(resource);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAll()
        {
            var users = await _userRepository.GetUsersByRoleAsync("User");
            if (users == null || !users.Any())
                return NotFound(new { Errors = "No users were found." });

            var resources = _mapper.Map<IEnumerable<UserResource>>(users);

            return Ok(resources);
        }

        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequest request)
        {
            var entityId = this.User.GetUserId();

            var user = await _userManager.FindByIdAsync(entityId.ToString());
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

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string userEmail)
        {
            var userToDelete = await _userRepository.GetUserByEmail(userEmail);

            if (userToDelete == null)
                return NotFound("User not found.");

            // Remove related entities
            await _userRepository.DeleteUserProjectsAndStandardsAsync(userToDelete.Id);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Delete the user
            var result = await _userManager.DeleteAsync(userToDelete);

            if (result.Succeeded)
                return Ok();
            else
                return BadRequest("Error deleting user.");
        }
    }
}
