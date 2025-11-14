using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PlanStack.Backend.Database.DataModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanStack.Backend.WebAPI.Handlers
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IConfigurationSection _jwtSettings;

        public JwtHandler(
            IConfiguration configuration,
            UserManager<User> userManager
        )
        {
            _configuration = configuration;
            _userManager = userManager;
            _jwtSettings = _configuration.GetSection("JwtSettings");
        }

        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),         
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), 
                new Claim(JwtRegisteredClaimNames.Email, user.Email),    
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            return claims;
        }

        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }
    }
}
