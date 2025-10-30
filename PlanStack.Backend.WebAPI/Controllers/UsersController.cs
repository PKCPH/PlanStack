using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Backend.WebAPI.Handlers;
using System.IdentityModel.Tokens.Jwt;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly UserRepository _userRepository;

        public UsersController(
            UserManager<User> userManager,
            IMapper mapper,
            JwtHandler jwtHandler,
            UserRepository userRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationPostRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest();

            if (string.IsNullOrEmpty(request.Password))
                return BadRequest();

            if (!request.Email.Contains("@"))
                return BadRequest();

            var user = _mapper.Map<User>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new UserRegistrationPostResponse { Errors = errors });
            }

            await _userManager.AddToRoleAsync(user, "User");

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginPostRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized(new UserLoginPostResponse { ErrorMessage = "Invalid Authentication - Check Email or Password" });

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new UserLoginPostResponse { IsAuthSuccessful = true, Token = token });
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
