using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(DatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            var roleId = _context.Roles.Where(x => x.Name == roleName).FirstOrDefault().Id;

            return await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
                //.Include(x => x.Stats)
                .ToListAsync();
        }
    }
}
