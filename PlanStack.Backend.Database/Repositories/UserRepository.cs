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

        public async Task DeleteUserProjectsAndStandardsAsync(string userId)
        {
            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();
            
            var standards = await _context.Standards
                .Where(s => s.UserId == userId)
                .ToListAsync();

            _context.Projects.RemoveRange(projects);
            _context.Standards.RemoveRange(standards);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Include(x => x.Projects)
                .Include(x => x.Standards)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            var roleId = _context.Roles.Where(x => x.Name == roleName).FirstOrDefault().Id;

            return await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
                .Include(x => x.Projects)
                .Include(x => x.Standards)
                .ToListAsync();
        }
    }
}
