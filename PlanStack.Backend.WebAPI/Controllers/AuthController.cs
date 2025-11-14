using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Backend.WebAPI.Handlers;
using System.IdentityModel.Tokens.Jwt;

namespace PlanStack.Backend.WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;

        public AuthController(
            UserManager<User> userManager,
            IMapper mapper,
            JwtHandler jwtHandler
        )
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
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
            var claims = await _jwtHandler.GetClaimsAsync(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new UserLoginPostResponse { IsAuthSuccessful = true, Token = token });
        }
    }
}
